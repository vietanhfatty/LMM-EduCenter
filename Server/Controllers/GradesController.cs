using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Grade;
using Server.Models;

namespace Server.Controllers;

[Route("api/grades")]
[ApiController]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly LMMDbContext _db;

    public GradesController(LMMDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<ActionResult<List<GradeDto>>> GetByClass([FromQuery] int classId)
    {
        var list = await _db.Grades
            .Include(g => g.Student)
            .Include(g => g.Class).ThenInclude(c => c.Course)
            .Where(g => g.ClassId == classId)
            .OrderBy(g => g.Student.FullName).ThenBy(g => g.Type)
            .Select(g => MapGrade(g))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<StudentGradeSummaryDto>>> GetMy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollments = await _db.Enrollments
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .Where(e => e.StudentId == userId && e.Status == (int)EnrollmentStatus.Approved)
            .ToListAsync();

        var grades = await _db.Grades
            .Include(g => g.Class).ThenInclude(c => c.Course)
            .Where(g => g.StudentId == userId)
            .ToListAsync();

        var selectedEnrollments = enrollments
            .GroupBy(e => e.Class.CourseId)
            .Select(g => g
                .OrderByDescending(e => GetClassPriority(e.Class.Status))
                .ThenByDescending(e => e.EnrollDate)
                .First())
            .ToList();

        var summaries = selectedEnrollments.Select(e =>
        {
            var classGrades = grades.Where(g => g.ClassId == e.ClassId).ToList();
            double? avg = null;
            if (classGrades.Any())
            {
                var totalWeight = classGrades.Sum(g => g.Weight);
                if (totalWeight > 0)
                    avg = Math.Round(classGrades.Sum(g => g.Score * g.Weight) / totalWeight, 2);
            }

            return new StudentGradeSummaryDto
            {
                ClassId = e.ClassId,
                ClassName = e.Class.Name,
                CourseName = e.Class.Course.Name,
                Grades = classGrades.Select(g => new GradeDto
                {
                    Id = g.Id,
                    StudentId = g.StudentId,
                    ClassId = g.ClassId,
                    ClassName = g.Class.Name,
                    CourseName = g.Class.Course.Name,
                    Type = g.Type,
                    Description = g.Description,
                    Score = g.Score,
                    Weight = g.Weight,
                    CreatedAt = g.CreatedAt
                }).OrderBy(g => g.Type).ToList(),
                WeightedAverage = avg
            };
        }).ToList();

        return Ok(summaries);
    }

    [HttpPost("batch")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> BatchCreate([FromBody] BatchGradeDto dto)
    {
        foreach (var item in dto.Items)
        {
            var existing = await _db.Grades.FirstOrDefaultAsync(g =>
                g.ClassId == dto.ClassId && g.StudentId == item.StudentId && g.Type == dto.Type);

            if (existing != null)
            {
                existing.Score = item.Score;
                existing.Description = dto.Description;
                existing.Weight = dto.Weight;
            }
            else
            {
                _db.Grades.Add(new Grade
                {
                    ClassId = dto.ClassId,
                    StudentId = item.StudentId,
                    Type = dto.Type,
                    Description = dto.Description,
                    Score = item.Score,
                    Weight = dto.Weight,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Lưu điểm thành công." });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Update(int id, [FromBody] GradeItemDto dto)
    {
        var grade = await _db.Grades.FindAsync(id);
        if (grade == null) return NotFound();

        grade.Score = dto.Score;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Cập nhật điểm thành công." });
    }

    private static GradeDto MapGrade(Grade g) => new()
    {
        Id = g.Id,
        StudentId = g.StudentId,
        StudentName = g.Student?.FullName ?? "",
        ClassId = g.ClassId,
        ClassName = g.Class?.Name ?? "",
        CourseName = g.Class?.Course?.Name ?? "",
        Type = g.Type,
        Description = g.Description,
        Score = g.Score,
        Weight = g.Weight,
        CreatedAt = g.CreatedAt
    };

    private static int GetClassPriority(int classStatus) => classStatus switch
    {
        (int)ClassStatus.InProgress => 3,
        (int)ClassStatus.Upcoming => 2,
        (int)ClassStatus.Completed => 1,
        _ => 0
    };
}
