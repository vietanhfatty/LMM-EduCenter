using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Attendance;
using Server.Models;

namespace Server.Controllers;

[Route("api/attendances")]
[ApiController]
[Authorize]
public class AttendancesController : ControllerBase
{
    private readonly LMMDbContext _db;

    public AttendancesController(LMMDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<ActionResult<List<AttendanceDto>>> GetByClassAndDate(
        [FromQuery] int classId, [FromQuery] DateTime? date = null)
    {
        if (!await CanAccessClassAsync(classId))
            return Forbid();

        var query = _db.Attendances
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Where(a => a.ClassId == classId);

        if (date.HasValue)
            query = query.Where(a => a.Date.Date == date.Value.Date);

        var list = await query
            .OrderBy(a => a.Student.FullName)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                ClassId = a.ClassId,
                ClassName = a.Class.Name,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                Date = a.Date,
                Status = a.Status,
                Note = a.Note
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<AttendanceDto>>> GetMy([FromQuery] int? classId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = _db.Attendances
            .Include(a => a.Class)
            .Where(a => a.StudentId == userId);

        if (classId.HasValue)
            query = query.Where(a => a.ClassId == classId.Value);

        var list = await query
            .OrderByDescending(a => a.Date)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                ClassId = a.ClassId,
                ClassName = a.Class.Name,
                StudentId = a.StudentId,
                StudentName = "",
                Date = a.Date,
                Status = a.Status,
                Note = a.Note
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpPost("batch")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> BatchCreate([FromBody] BatchAttendanceDto dto)
    {
        var classEntity = await _db.Classes
            .Include(c => c.ClassSchedules)
            .FirstOrDefaultAsync(c => c.Id == dto.ClassId);
        if (classEntity == null)
            return NotFound(new { message = "Không tìm thấy lớp học." });

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (classEntity.TeacherId != teacherId)
            return Forbid();

        var attendanceDate = dto.Date.Date;
        if (attendanceDate < classEntity.StartDate.Date || attendanceDate > classEntity.EndDate.Date)
            return BadRequest(new { message = "Ngày điểm danh nằm ngoài thời gian diễn ra của lớp học." });

        var scheduleDays = classEntity.ClassSchedules
            .Select(s => NormalizeDayOfWeek(s.DayOfWeek))
            .Distinct()
            .ToList();
        if (!scheduleDays.Contains(NormalizeDayOfWeek((int)attendanceDate.DayOfWeek)))
            return BadRequest(new { message = "Ngày đã chọn không trùng với lịch học của lớp." });

        var approvedStudentIds = await _db.Enrollments
            .Where(e => e.ClassId == dto.ClassId && e.Status == (int)EnrollmentStatus.Approved)
            .Select(e => e.StudentId)
            .ToListAsync();
        var approvedStudentSet = approvedStudentIds.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in dto.Items)
        {
            if (!approvedStudentSet.Contains(item.StudentId))
                continue;

            var existing = await _db.Attendances.FirstOrDefaultAsync(a =>
                a.ClassId == dto.ClassId && a.StudentId == item.StudentId && a.Date.Date == dto.Date.Date);

            if (existing != null)
            {
                existing.Status = item.Status;
                existing.Note = item.Note;
            }
            else
            {
                _db.Attendances.Add(new Attendance
                {
                    ClassId = dto.ClassId,
                    StudentId = item.StudentId,
                    Date = dto.Date.Date,
                    Status = item.Status,
                    Note = item.Note
                });
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Điểm danh thành công." });
    }

    [HttpGet("report/{classId}")]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<ActionResult<List<AttendanceReportDto>>> GetReport(int classId)
    {
        var students = await _db.Enrollments
            .Where(e => e.ClassId == classId && e.Status == (int)EnrollmentStatus.Approved)
            .Include(e => e.Student)
            .Select(e => new { e.StudentId, e.Student.FullName })
            .ToListAsync();

        var attendances = await _db.Attendances
            .Where(a => a.ClassId == classId)
            .ToListAsync();

        var totalDates = attendances.Select(a => a.Date.Date).Distinct().Count();

        var report = students.Select(s =>
        {
            var records = attendances.Where(a => a.StudentId == s.StudentId).ToList();
            var present = records.Count(r => r.Status == (int)AttendanceStatus.Present);
            var late = records.Count(r => r.Status == (int)AttendanceStatus.Late);
            var absent = records.Count(r => r.Status == (int)AttendanceStatus.Absent);
            var excused = records.Count(r => r.Status == (int)AttendanceStatus.Excused);

            return new AttendanceReportDto
            {
                StudentId = s.StudentId,
                StudentName = s.FullName,
                TotalSessions = totalDates,
                Present = present,
                Late = late,
                Absent = absent,
                Excused = excused,
                AttendanceRate = totalDates > 0 ? Math.Round((present + late) * 100.0 / totalDates, 1) : 0
            };
        }).ToList();

        return Ok(report);
    }

    [HttpGet("dates/{classId}")]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<ActionResult<List<DateTime>>> GetDates(int classId)
    {
        var dates = await _db.Attendances
            .Where(a => a.ClassId == classId)
            .Select(a => a.Date.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync();

        return Ok(dates);
    }

    private async Task<bool> CanAccessClassAsync(int classId)
    {
        if (User.IsInRole("Staff"))
            return true;

        if (!User.IsInRole("Teacher"))
            return false;

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(teacherId))
            return false;

        return await _db.Classes.AnyAsync(c => c.Id == classId && c.TeacherId == teacherId);
    }

    private static int NormalizeDayOfWeek(int dayOfWeek)
    {
        var normalized = dayOfWeek % 7;
        if (normalized < 0) normalized += 7;
        return normalized;
    }
}
