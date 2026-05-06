using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Server.Data;
using Server.DTOs.Class;
using Server.Models;

namespace Server.Controllers;

[Route("api/classes")]
[ApiController]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly LMMDbContext _db;

    public ClassesController(LMMDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ClassDto>>> GetAll([FromQuery] string? teacherId = null)
    {
        var query = _db.Classes
            .Include(c => c.Course)
            .Include(c => c.Teacher)
            .Include(c => c.Room)
            .Include(c => c.ClassSchedules)
            .Include(c => c.Enrollments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(teacherId))
            query = query.Where(c => c.TeacherId == teacherId);

        var list = await query
            .OrderByDescending(c => c.StartDate)
            .Select(c => MapClass(c))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ClassDto>> GetById(int id)
    {
        var c = await _db.Classes
            .Include(x => x.Course)
            .Include(x => x.Teacher)
            .Include(x => x.Room)
            .Include(x => x.ClassSchedules)
            .Include(x => x.Enrollments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null) return NotFound();
        return Ok(MapClass(c));
    }

    [HttpPost]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassDto dto)
    {
        var minStartDate = DateTime.Today.AddDays(7);

        if (dto.Schedules == null || dto.Schedules.Count == 0)
            return BadRequest(new { message = "Lớp học phải có ít nhất 1 lịch học." });

        if (dto.StartDate.Date < minStartDate)
            return BadRequest(new { message = "Ngày bắt đầu phải cách ngày hiện tại ít nhất 7 ngày." });

        if (dto.StartDate.Date > dto.EndDate.Date)
            return BadRequest(new { message = "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc." });

        var inputSchedules = ParseSchedules(dto.Schedules);
        if (inputSchedules == null)
            return BadRequest(new { message = "Giờ học không hợp lệ. Chỉ chấp nhận 4 ca: 08:30-10:30, 13:30-15:30, 16:30-18:30, 19:00-21:00." });

        if (HasSelfScheduleConflict(inputSchedules))
            return BadRequest(new { message = "Lịch học bị trùng giờ trong cùng lớp." });

        var classesInRange = await _db.Classes
            .Include(c => c.ClassSchedules)
            .Where(c => (c.Status == (int)ClassStatus.Upcoming || c.Status == (int)ClassStatus.InProgress)
                        && c.StartDate.Date <= dto.EndDate.Date
                        && c.EndDate.Date >= dto.StartDate.Date)
            .ToListAsync();

        var roomConflict = classesInRange.FirstOrDefault(c =>
            c.RoomId == dto.RoomId && HasScheduleConflict(inputSchedules, c.ClassSchedules));
        if (roomConflict != null)
            return BadRequest(new { message = $"Phòng học bị trùng lịch với lớp '{roomConflict.Name}'." });

        var teacherConflict = classesInRange.FirstOrDefault(c =>
            c.TeacherId == dto.TeacherId && HasScheduleConflict(inputSchedules, c.ClassSchedules));
        if (teacherConflict != null)
            return BadRequest(new { message = $"Giáo viên bị trùng lịch với lớp '{teacherConflict.Name}'." });

        var entity = new Class
        {
            Name = dto.Name,
            CourseId = dto.CourseId,
            TeacherId = dto.TeacherId,
            RoomId = dto.RoomId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxStudents = dto.MaxStudents,
            Status = dto.Status,
            ClassSchedules = inputSchedules.Select(s => new ClassSchedule
            {
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList()
        };

        _db.Classes.Add(entity);
        await _db.SaveChangesAsync();

        var created = await _db.Classes
            .Include(x => x.Course).Include(x => x.Teacher)
            .Include(x => x.Room).Include(x => x.ClassSchedules)
            .Include(x => x.Enrollments)
            .FirstAsync(x => x.Id == entity.Id);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapClass(created));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<ClassDto>> Update(int id, [FromBody] UpdateClassDto dto)
    {
        var minStartDate = DateTime.Today.AddDays(7);

        if (dto.Schedules == null || dto.Schedules.Count == 0)
            return BadRequest(new { message = "Lớp học phải có ít nhất 1 lịch học." });

        if (dto.StartDate.Date < minStartDate)
            return BadRequest(new { message = "Ngày bắt đầu phải cách ngày hiện tại ít nhất 7 ngày." });

        if (dto.StartDate.Date > dto.EndDate.Date)
            return BadRequest(new { message = "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc." });

        var inputSchedules = ParseSchedules(dto.Schedules);
        if (inputSchedules == null)
            return BadRequest(new { message = "Giờ học không hợp lệ. Chỉ chấp nhận 4 ca: 08:30-10:30, 13:30-15:30, 16:30-18:30, 19:00-21:00." });

        if (HasSelfScheduleConflict(inputSchedules))
            return BadRequest(new { message = "Lịch học bị trùng giờ trong cùng lớp." });

        var entity = await _db.Classes
            .Include(x => x.ClassSchedules)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return NotFound();

        var classesInRange = await _db.Classes
            .Include(c => c.ClassSchedules)
            .Where(c => c.Id != id
                        && (c.Status == (int)ClassStatus.Upcoming || c.Status == (int)ClassStatus.InProgress)
                        && c.StartDate.Date <= dto.EndDate.Date
                        && c.EndDate.Date >= dto.StartDate.Date)
            .ToListAsync();

        var roomConflict = classesInRange.FirstOrDefault(c =>
            c.RoomId == dto.RoomId && HasScheduleConflict(inputSchedules, c.ClassSchedules));
        if (roomConflict != null)
            return BadRequest(new { message = $"Phòng học bị trùng lịch với lớp '{roomConflict.Name}'." });

        var teacherConflict = classesInRange.FirstOrDefault(c =>
            c.TeacherId == dto.TeacherId && HasScheduleConflict(inputSchedules, c.ClassSchedules));
        if (teacherConflict != null)
            return BadRequest(new { message = $"Giáo viên bị trùng lịch với lớp '{teacherConflict.Name}'." });

        entity.Name = dto.Name;
        entity.CourseId = dto.CourseId;
        entity.TeacherId = dto.TeacherId;
        entity.RoomId = dto.RoomId;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.MaxStudents = dto.MaxStudents;
        entity.Status = dto.Status;

        _db.ClassSchedules.RemoveRange(entity.ClassSchedules);
        entity.ClassSchedules = inputSchedules.Select(s => new ClassSchedule
        {
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();

        await _db.SaveChangesAsync();

        var updated = await _db.Classes
            .Include(x => x.Course).Include(x => x.Teacher)
            .Include(x => x.Room).Include(x => x.ClassSchedules)
            .Include(x => x.Enrollments)
            .FirstAsync(x => x.Id == id);

        return Ok(MapClass(updated));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Classes
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null) return NotFound();
        if (entity.Enrollments.Any())
            return BadRequest(new { message = "Không thể xóa lớp học đã có học viên đăng ký." });

        _db.Classes.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/students")]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<IActionResult> GetStudents(int id)
    {
        if (User.IsInRole("Teacher"))
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var canAccess = await _db.Classes.AnyAsync(c => c.Id == id && c.TeacherId == teacherId);
            if (!canAccess)
                return Forbid();
        }

        var students = await _db.Enrollments
            .Where(e => e.ClassId == id && e.Status == (int)EnrollmentStatus.Approved)
            .Include(e => e.Student)
            .Select(e => new
            {
                e.Student.Id,
                e.Student.FullName,
                e.Student.Email,
                e.Student.Phone,
                e.EnrollDate
            })
            .ToListAsync();

        return Ok(students);
    }

    private static ClassDto MapClass(Class c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        CourseId = c.CourseId,
        CourseName = c.Course?.Name ?? "",
        TeacherId = c.TeacherId,
        TeacherName = c.Teacher?.FullName ?? "",
        RoomId = c.RoomId,
        RoomName = c.Room?.Name ?? "",
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        MaxStudents = c.MaxStudents,
        CurrentStudents = c.Enrollments?.Count(e => e.Status == (int)EnrollmentStatus.Approved) ?? 0,
        Status = c.Status,
        Schedules = c.ClassSchedules?.Select(s => new ClassScheduleDto
        {
            Id = s.Id,
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime.ToString("HH:mm"),
            EndTime = s.EndTime.ToString("HH:mm")
        }).ToList() ?? new()
    };

    private static List<(int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime)>? ParseSchedules(IEnumerable<CreateScheduleDto> schedules)
    {
        var result = new List<(int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime)>();

        foreach (var s in schedules)
        {
            if (!TimeOnly.TryParse(s.StartTime, out var startTime) || !TimeOnly.TryParse(s.EndTime, out var endTime))
                return null;

            if (startTime >= endTime)
                return null;

            if (!IsHardSlot(startTime, endTime))
                return null;

            result.Add((s.DayOfWeek, startTime, endTime));
        }

        return result;
    }

    private static bool HasSelfScheduleConflict(List<(int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime)> schedules)
    {
        for (var i = 0; i < schedules.Count; i++)
        {
            for (var j = i + 1; j < schedules.Count; j++)
            {
                if (IsTimeOverlap(schedules[i], schedules[j]))
                    return true;
            }
        }
        return false;
    }

    private static bool HasScheduleConflict(
        List<(int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime)> inputSchedules,
        IEnumerable<ClassSchedule> existingSchedules)
    {
        foreach (var input in inputSchedules)
        {
            foreach (var existing in existingSchedules)
            {
                if (input.DayOfWeek != existing.DayOfWeek)
                    continue;

                if (input.StartTime < existing.EndTime && existing.StartTime < input.EndTime)
                    return true;
            }
        }

        return false;
    }

    private static bool IsTimeOverlap(
        (int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime) a,
        (int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime) b) =>
        a.DayOfWeek == b.DayOfWeek && a.StartTime < b.EndTime && b.StartTime < a.EndTime;

    private static bool IsHardSlot(TimeOnly start, TimeOnly end) =>
        (start == new TimeOnly(8, 30) && end == new TimeOnly(10, 30)) ||
        (start == new TimeOnly(13, 30) && end == new TimeOnly(15, 30)) ||
        (start == new TimeOnly(16, 30) && end == new TimeOnly(18, 30)) ||
        (start == new TimeOnly(19, 0) && end == new TimeOnly(21, 0));
}
