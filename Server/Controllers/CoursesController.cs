using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Course;
using Server.Models;

namespace Server.Controllers;

[Route("api/courses")]
[ApiController]
[Authorize(Roles = "Staff")]
public class CoursesController : ControllerBase
{
    private readonly LMMDbContext _db;

    public CoursesController(LMMDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CourseDto>>> GetAll()
    {
        var list = await _db.Courses
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Fee = c.Fee,
                DurationInHours = c.DurationInHours,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                ClassCount = c.Classes.Count,
                SubjectCount = c.Subjects.Count
            })
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var c = await _db.Courses
            .Where(x => x.Id == id)
            .Select(x => new CourseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Fee = x.Fee,
                DurationInHours = x.DurationInHours,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                ClassCount = x.Classes.Count,
                SubjectCount = x.Subjects.Count
            })
            .FirstOrDefaultAsync();

        if (c == null) return NotFound();
        return Ok(c);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
    {
        var entity = new Course
        {
            Name = dto.Name,
            Description = dto.Description,
            Fee = dto.Fee,
            DurationInHours = dto.DurationInHours,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Courses.Add(entity);
        await _db.SaveChangesAsync();

        var result = new CourseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Fee = entity.Fee,
            DurationInHours = entity.DurationInHours,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> Update(int id, [FromBody] UpdateCourseDto dto)
    {
        var entity = await _db.Courses.FindAsync(id);
        if (entity == null) return NotFound();

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Fee = dto.Fee;
        entity.DurationInHours = dto.DurationInHours;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        var result = new CourseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Fee = entity.Fee,
            DurationInHours = entity.DurationInHours,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Courses.FindAsync(id);
        if (entity == null) return NotFound();

        var hasClasses = await _db.Classes.AnyAsync(c => c.CourseId == id);
        if (hasClasses)
            return BadRequest(new { message = "Không thể xóa khóa học đã có lớp học." });

        _db.Courses.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
