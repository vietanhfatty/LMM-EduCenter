using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Enrollment;
using Server.Models;

namespace Server.Controllers;

[Route("api/enrollments")]
[ApiController]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly LMMDbContext _db;

    public EnrollmentsController(LMMDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetAll([FromQuery] int? status = null)
    {
        var query = _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .Include(e => e.Class).ThenInclude(c => c.Teacher)
            .Include(e => e.Class).ThenInclude(c => c.ClassSchedules)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        var list = await query
            .OrderByDescending(e => e.EnrollDate)
            .Select(e => MapEnrollment(e))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetMy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var list = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .Include(e => e.Class).ThenInclude(c => c.Teacher)
            .Include(e => e.Class).ThenInclude(c => c.ClassSchedules)
            .Include(e => e.Class).ThenInclude(c => c.Enrollments)
            .Where(e => e.StudentId == userId)
            .OrderByDescending(e => e.EnrollDate)
            .Select(e => MapEnrollment(e))
            .ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<EnrollmentDto>> Create([FromBody] CreateEnrollmentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cls = await _db.Classes
            .Include(c => c.Course)
            .Include(c => c.Enrollments)
            .Include(c => c.ClassSchedules)
            .FirstOrDefaultAsync(c => c.Id == dto.ClassId);

        if (cls == null)
            return NotFound(new { message = "Lớp học không tồn tại." });

        if (cls.Status != (int)ClassStatus.Upcoming && cls.Status != (int)ClassStatus.InProgress)
            return BadRequest(new { message = "Lớp học không còn mở đăng ký." });

        var approvedCount = cls.Enrollments.Count(e => e.Status == (int)EnrollmentStatus.Approved);
        if (approvedCount >= cls.MaxStudents)
            return BadRequest(new { message = "Lớp học đã đủ sĩ số." });

        var exists = await _db.Enrollments.AnyAsync(e =>
            e.StudentId == userId && e.ClassId == dto.ClassId
            && (e.Status == (int)EnrollmentStatus.Pending || e.Status == (int)EnrollmentStatus.Approved));
        if (exists)
            return BadRequest(new { message = "Bạn đã đăng ký lớp học này rồi." });

        var hasAnotherActiveEnrollmentInCourse = await _db.Enrollments.AnyAsync(e =>
            e.StudentId == userId
            && e.Class.CourseId == cls.CourseId
            && e.ClassId != dto.ClassId
            && (e.Status == (int)EnrollmentStatus.Pending || e.Status == (int)EnrollmentStatus.Approved)
            && e.Class.Status != (int)ClassStatus.Completed
            && e.Class.Status != (int)ClassStatus.Cancelled);
        if (hasAnotherActiveEnrollmentInCourse)
            return BadRequest(new { message = "Mỗi khóa học chỉ được đăng ký 1 lớp cho đến khi lớp hiện tại kết thúc." });

        var studentActiveEnrollments = await _db.Enrollments
            .Include(e => e.Class)
            .ThenInclude(c => c.ClassSchedules)
            .Where(e => e.StudentId == userId
                        && e.ClassId != dto.ClassId
                        && (e.Status == (int)EnrollmentStatus.Pending || e.Status == (int)EnrollmentStatus.Approved)
                        && e.Class.Status != (int)ClassStatus.Completed
                        && e.Class.Status != (int)ClassStatus.Cancelled
                        && e.Class.StartDate.Date <= cls.EndDate.Date
                        && e.Class.EndDate.Date >= cls.StartDate.Date)
            .ToListAsync();

        var hasTimeConflict = studentActiveEnrollments.Any(e =>
            HasScheduleConflict(cls.ClassSchedules, e.Class.ClassSchedules));
        if (hasTimeConflict)
            return BadRequest(new { message = "Bạn đã đăng ký lớp khác bị trùng lịch học." });

        var enrollment = new Enrollment
        {
            StudentId = userId!,
            ClassId = dto.ClassId,
            EnrollDate = DateTime.UtcNow,
            Status = (int)EnrollmentStatus.Pending
        };

        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();

        var created = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .Include(e => e.Class).ThenInclude(c => c.Teacher)
            .Include(e => e.Class).ThenInclude(c => c.ClassSchedules)
            .Include(e => e.Class).ThenInclude(c => c.Enrollments)
            .FirstAsync(e => e.Id == enrollment.Id);

        return CreatedAtAction(nameof(GetMy), MapEnrollment(created));
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Approve(int id)
    {
        var enrollment = await _db.Enrollments
            .Include(e => e.Class)
            .ThenInclude(c => c.Course)
            .Include(e => e.Class)
            .ThenInclude(c => c.ClassSchedules)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null) return NotFound();

        if (enrollment.Status != (int)EnrollmentStatus.Pending)
            return BadRequest(new { message = "Chỉ có thể duyệt đăng ký đang chờ xử lý." });

        var approvedCount = await _db.Enrollments.CountAsync(e =>
            e.ClassId == enrollment.ClassId && e.Status == (int)EnrollmentStatus.Approved);
        if (approvedCount >= enrollment.Class.MaxStudents)
            return BadRequest(new { message = "Lớp học đã đủ sĩ số." });

        var hasAnotherApprovedInCourse = await _db.Enrollments.AnyAsync(e =>
            e.Id != enrollment.Id
            && e.StudentId == enrollment.StudentId
            && e.Status == (int)EnrollmentStatus.Approved
            && e.Class.CourseId == enrollment.Class.CourseId
            && e.Class.Status != (int)ClassStatus.Completed
            && e.Class.Status != (int)ClassStatus.Cancelled);
        if (hasAnotherApprovedInCourse)
            return BadRequest(new { message = "Học viên đã có lớp được duyệt trong khóa học này." });

        var approvedEnrollments = await _db.Enrollments
            .Include(e => e.Class)
            .ThenInclude(c => c.ClassSchedules)
            .Where(e =>
                e.Id != enrollment.Id
                && e.StudentId == enrollment.StudentId
                && e.Status == (int)EnrollmentStatus.Approved
                && e.Class.Status != (int)ClassStatus.Completed
                && e.Class.Status != (int)ClassStatus.Cancelled
                && e.Class.StartDate.Date <= enrollment.Class.EndDate.Date
                && e.Class.EndDate.Date >= enrollment.Class.StartDate.Date)
            .ToListAsync();

        var hasTimeConflict = approvedEnrollments.Any(e =>
            HasScheduleConflict(enrollment.Class.ClassSchedules, e.Class.ClassSchedules));
        if (hasTimeConflict)
            return BadRequest(new { message = "Học viên đã có lớp khác trùng ca học trong cùng ngày." });

        enrollment.Status = (int)EnrollmentStatus.Approved;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Duyệt đăng ký thành công." });
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Reject(int id)
    {
        var enrollment = await _db.Enrollments.FindAsync(id);
        if (enrollment == null) return NotFound();

        if (enrollment.Status != (int)EnrollmentStatus.Pending)
            return BadRequest(new { message = "Chỉ có thể từ chối đăng ký đang chờ xử lý." });

        enrollment.Status = (int)EnrollmentStatus.Rejected;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Đã từ chối đăng ký." });
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var enrollment = await _db.Enrollments.FindAsync(id);
        if (enrollment == null) return NotFound();

        if (enrollment.StudentId != userId)
            return Forbid();

        if (enrollment.Status != (int)EnrollmentStatus.Pending)
            return BadRequest(new { message = "Chỉ có thể hủy đăng ký đang chờ xử lý." });

        enrollment.Status = (int)EnrollmentStatus.Cancelled;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Đã hủy đăng ký." });
    }

    private static EnrollmentDto MapEnrollment(Enrollment e) => new()
    {
        Id = e.Id,
        StudentId = e.StudentId,
        StudentName = e.Student?.FullName ?? "",
        StudentEmail = e.Student?.Email ?? "",
        ClassId = e.ClassId,
        ClassName = e.Class?.Name ?? "",
        CourseName = e.Class?.Course?.Name ?? "",
        TeacherId = e.Class?.TeacherId ?? "",
        TeacherName = e.Class?.Teacher?.FullName ?? "",
        EnrollDate = e.EnrollDate,
        Status = e.Status,
        MaxStudents = e.Class?.MaxStudents ?? 0,
        CurrentStudents = e.Class?.Enrollments?.Count(x => x.Status == (int)EnrollmentStatus.Approved) ?? 0,
        Schedules = e.Class?.ClassSchedules?.Select(s => new EnrollmentScheduleDto
        {
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime.ToString("HH:mm"),
            EndTime = s.EndTime.ToString("HH:mm")
        }).ToList() ?? new()
    };

    private static bool HasScheduleConflict(IEnumerable<ClassSchedule> first, IEnumerable<ClassSchedule> second)
    {
        foreach (var a in first)
        {
            foreach (var b in second)
            {
                if (a.DayOfWeek != b.DayOfWeek)
                    continue;

                if (a.StartTime < b.EndTime && b.StartTime < a.EndTime)
                    return true;
            }
        }

        return false;
    }
}
