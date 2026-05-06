using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Payment;
using Server.Models;

namespace Server.Controllers;

[Route("api/payments")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly LMMDbContext _db;

    public PaymentsController(LMMDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<List<PaymentDto>>> GetAll()
    {
        var list = await _db.Payments
            .Include(p => p.Student)
            .Include(p => p.Class).ThenInclude(c => c.Course)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => MapPayment(p))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<PaymentDto>>> GetMy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var list = await _db.Payments
            .Include(p => p.Student)
            .Include(p => p.Class).ThenInclude(c => c.Course)
            .Where(p => p.StudentId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => MapPayment(p))
            .ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto)
    {
        if (dto.Method != (int)PaymentMethod.Cash)
            return BadRequest(new { message = "Staff chỉ được tạo thanh toán tiền mặt tại trung tâm." });

        var enrollment = await _db.Enrollments
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .FirstOrDefaultAsync(e =>
                e.StudentId == dto.StudentId &&
                e.ClassId == dto.ClassId &&
                e.Status == (int)EnrollmentStatus.Approved);

        if (enrollment is null)
            return BadRequest(new { message = "Học viên chưa có đăng ký được duyệt cho lớp này." });

        var paid = await GetCompletedPaidAmount(dto.StudentId, dto.ClassId);
        var remaining = enrollment.Class.Course.Fee - paid;

        if (remaining <= 0)
            return BadRequest(new { message = "Học viên đã hoàn thành học phí lớp này." });

        if (dto.Amount <= 0 || dto.Amount > remaining)
            return BadRequest(new { message = $"Số tiền không hợp lệ. Còn lại {remaining:N0} VND." });

        var entity = new Payment
        {
            StudentId = dto.StudentId,
            ClassId = dto.ClassId,
            Amount = dto.Amount,
            Method = (int)PaymentMethod.Cash,
            Status = (int)PaymentStatus.Completed,
            PaymentDate = DateTime.UtcNow,
            Note = dto.Note
        };

        _db.Payments.Add(entity);
        await _db.SaveChangesAsync();

        var created = await _db.Payments
            .Include(p => p.Student)
            .Include(p => p.Class).ThenInclude(c => c.Course)
            .FirstAsync(p => p.Id == entity.Id);

        return CreatedAtAction(nameof(GetAll), MapPayment(created));
    }

    [HttpPost("my/bank-transfer")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<PaymentDto>> CreateMyBankTransfer([FromBody] CreateBankTransferPaymentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var enrollment = await _db.Enrollments
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .FirstOrDefaultAsync(e =>
                e.StudentId == userId &&
                e.ClassId == dto.ClassId &&
                e.Status == (int)EnrollmentStatus.Approved);

        if (enrollment is null)
            return BadRequest(new { message = "Bạn chỉ có thể chuyển khoản cho lớp đã được duyệt đăng ký." });

        var paid = await GetCompletedPaidAmount(userId, dto.ClassId);
        var pendingTransfer = await _db.Payments
            .Where(p => p.StudentId == userId &&
                        p.ClassId == dto.ClassId &&
                        p.Method == (int)PaymentMethod.BankTransfer &&
                        p.Status == (int)PaymentStatus.Pending)
            .SumAsync(p => p.Amount);

        var remaining = enrollment.Class.Course.Fee - paid - pendingTransfer;
        if (remaining <= 0)
            return BadRequest(new { message = "Lớp này không còn công nợ hoặc đã có yêu cầu chờ xác nhận." });

        if (dto.Amount <= 0 || dto.Amount > remaining)
            return BadRequest(new { message = $"Số tiền chuyển khoản không hợp lệ. Tối đa {remaining:N0} VND." });

        var entity = new Payment
        {
            StudentId = userId,
            ClassId = dto.ClassId,
            Amount = dto.Amount,
            Method = (int)PaymentMethod.BankTransfer,
            Status = (int)PaymentStatus.Pending,
            PaymentDate = DateTime.UtcNow,
            Note = dto.Note
        };

        _db.Payments.Add(entity);
        await _db.SaveChangesAsync();

        var created = await _db.Payments
            .Include(p => p.Student)
            .Include(p => p.Class).ThenInclude(c => c.Course)
            .FirstAsync(p => p.Id == entity.Id);

        return CreatedAtAction(nameof(GetMy), MapPayment(created));
    }

    [HttpPost("{id:int}/confirm-transfer")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<PaymentDto>> ConfirmTransfer(int id)
    {
        var payment = await _db.Payments
            .Include(p => p.Student)
            .Include(p => p.Class).ThenInclude(c => c.Course)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null)
            return NotFound(new { message = "Không tìm thấy giao dịch." });

        if (payment.Method != (int)PaymentMethod.BankTransfer)
            return BadRequest(new { message = "Chỉ xác nhận giao dịch chuyển khoản." });

        if (payment.Status != (int)PaymentStatus.Pending)
            return BadRequest(new { message = "Giao dịch này không ở trạng thái chờ xác nhận." });

        var paid = await GetCompletedPaidAmount(payment.StudentId, payment.ClassId);
        var classFee = await _db.Classes
            .Where(c => c.Id == payment.ClassId)
            .Select(c => c.Course.Fee)
            .FirstAsync();

        var remaining = classFee - paid;
        if (payment.Amount > remaining)
            return BadRequest(new { message = $"Số tiền giao dịch vượt công nợ hiện tại ({remaining:N0} VND)." });

        payment.Status = (int)PaymentStatus.Completed;
        payment.PaymentDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(MapPayment(payment));
    }

    [HttpGet("debts")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<List<DebtDto>>> GetDebts()
    {
        var enrollments = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .Where(e => e.Status == (int)EnrollmentStatus.Approved)
            .ToListAsync();

        var payments = await _db.Payments
            .Where(p => p.Status == (int)PaymentStatus.Completed)
            .ToListAsync();

        var debts = enrollments.Select(e =>
        {
            var totalPaid = payments
                .Where(p => p.StudentId == e.StudentId && p.ClassId == e.ClassId)
                .Sum(p => p.Amount);

            return new DebtDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student.FullName,
                StudentEmail = e.Student.Email ?? "",
                ClassId = e.ClassId,
                ClassName = e.Class.Name,
                CourseFee = e.Class.Course.Fee,
                TotalPaid = totalPaid,
                Remaining = e.Class.Course.Fee - totalPaid
            };
        })
        .Where(d => d.Remaining > 0)
        .OrderByDescending(d => d.Remaining)
        .ToList();

        return Ok(debts);
    }

    [HttpGet("my/debts")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<DebtDto>>> GetMyDebts()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollments = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .Where(e => e.StudentId == userId && e.Status == (int)EnrollmentStatus.Approved)
            .ToListAsync();

        var payments = await _db.Payments
            .Where(p => p.StudentId == userId && p.Status == (int)PaymentStatus.Completed)
            .ToListAsync();

        var debts = enrollments.Select(e =>
        {
            var totalPaid = payments.Where(p => p.ClassId == e.ClassId).Sum(p => p.Amount);
            return new DebtDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student.FullName,
                StudentEmail = e.Student.Email ?? "",
                ClassId = e.ClassId,
                ClassName = e.Class.Name,
                CourseFee = e.Class.Course.Fee,
                TotalPaid = totalPaid,
                Remaining = e.Class.Course.Fee - totalPaid
            };
        })
        .Where(d => d.Remaining > 0)
        .OrderByDescending(d => d.Remaining)
        .ToList();

        return Ok(debts);
    }

    private async Task<decimal> GetCompletedPaidAmount(string studentId, int classId)
    {
        return await _db.Payments
            .Where(p => p.StudentId == studentId &&
                        p.ClassId == classId &&
                        p.Status == (int)PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }

    private static PaymentDto MapPayment(Payment p) => new()
    {
        Id = p.Id,
        StudentId = p.StudentId,
        StudentName = p.Student?.FullName ?? "",
        StudentEmail = p.Student?.Email ?? "",
        ClassId = p.ClassId,
        ClassName = p.Class?.Name ?? "",
        CourseName = p.Class?.Course?.Name ?? "",
        Amount = p.Amount,
        Method = p.Method,
        Status = p.Status,
        PaymentDate = p.PaymentDate,
        Note = p.Note
    };
}
