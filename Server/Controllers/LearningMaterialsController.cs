using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.LearningMaterial;
using Server.Models;

namespace Server.Controllers;

[Route("api/learning-materials")]
[ApiController]
[Authorize]
public class LearningMaterialsController : ControllerBase
{
    private readonly LMMDbContext _db;
    private readonly IWebHostEnvironment _env;

    public LearningMaterialsController(LMMDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<List<LearningMaterialDto>>> GetTeacherMaterials([FromQuery] int? classId = null)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(teacherId))
            return Unauthorized();

        var query = _db.LearningMaterials
            .Include(m => m.Class)
            .Include(m => m.Teacher)
            .Where(m => m.TeacherId == teacherId);

        if (classId.HasValue)
            query = query.Where(m => m.ClassId == classId.Value);

        var list = await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new LearningMaterialDto
            {
                Id = m.Id,
                ClassId = m.ClassId,
                ClassName = m.Class.Name,
                TeacherId = m.TeacherId,
                TeacherName = m.Teacher.FullName,
                Title = m.Title,
                Description = m.Description,
                FilePath = m.FilePath,
                OriginalFileName = m.OriginalFileName,
                FileSize = m.FileSize,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<LearningMaterialDto>>> GetStudentMaterials([FromQuery] int? classId = null)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(studentId))
            return Unauthorized();

        var approvedClassIds = _db.Enrollments
            .Where(e => e.StudentId == studentId && e.Status == (int)EnrollmentStatus.Approved)
            .Select(e => e.ClassId);

        var query = _db.LearningMaterials
            .Include(m => m.Class)
            .Include(m => m.Teacher)
            .Where(m => approvedClassIds.Contains(m.ClassId));

        if (classId.HasValue)
            query = query.Where(m => m.ClassId == classId.Value);

        var list = await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new LearningMaterialDto
            {
                Id = m.Id,
                ClassId = m.ClassId,
                ClassName = m.Class.Name,
                TeacherId = m.TeacherId,
                TeacherName = m.Teacher.FullName,
                Title = m.Title,
                Description = m.Description,
                FilePath = m.FilePath,
                OriginalFileName = m.OriginalFileName,
                FileSize = m.FileSize,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<LearningMaterialDto>> Upload([FromForm] CreateLearningMaterialForm form)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest(new { message = "Vui lòng chọn file tài liệu." });

        if (form.ClassId <= 0 || string.IsNullOrWhiteSpace(form.Title))
            return BadRequest(new { message = "Thiếu thông tin bắt buộc." });

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(teacherId))
            return Unauthorized();

        var classExists = await _db.Classes.AnyAsync(c => c.Id == form.ClassId && c.TeacherId == teacherId);
        if (!classExists)
            return BadRequest(new { message = "Bạn không có quyền gửi tài liệu cho lớp này." });

        var root = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(root))
            root = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        var folder = Path.Combine(root, "materials");
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(form.File.FileName);
        var safeExt = string.IsNullOrWhiteSpace(ext) ? ".bin" : ext;
        var storedName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{safeExt}";
        var fullPath = Path.Combine(folder, storedName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await form.File.CopyToAsync(stream);
        }

        var entity = new LearningMaterial
        {
            ClassId = form.ClassId,
            TeacherId = teacherId,
            Title = form.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
            FilePath = $"/materials/{storedName}",
            OriginalFileName = Path.GetFileName(form.File.FileName),
            FileSize = form.File.Length,
            CreatedAt = DateTime.UtcNow
        };

        _db.LearningMaterials.Add(entity);
        await _db.SaveChangesAsync();

        var created = await _db.LearningMaterials
            .Include(m => m.Class)
            .Include(m => m.Teacher)
            .FirstAsync(m => m.Id == entity.Id);

        return Ok(MapDto(created));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(teacherId))
            return Unauthorized();

        var entity = await _db.LearningMaterials.FirstOrDefaultAsync(m => m.Id == id && m.TeacherId == teacherId);
        if (entity == null)
            return NotFound();

        var root = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(root))
            root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        var file = Path.Combine(root, entity.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(file))
            System.IO.File.Delete(file);

        _db.LearningMaterials.Remove(entity);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Đã xóa tài liệu." });
    }

    [HttpGet("{id}/download")]
    [Authorize(Roles = "Teacher,Student")]
    public async Task<IActionResult> Download(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var entity = await _db.LearningMaterials
            .Include(m => m.Class)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null)
            return NotFound(new { message = "Không tìm thấy tài liệu." });

        if (User.IsInRole("Teacher"))
        {
            if (entity.TeacherId != userId)
                return Forbid();
        }
        else if (User.IsInRole("Student"))
        {
            var canAccess = await _db.Enrollments.AnyAsync(e =>
                e.StudentId == userId &&
                e.ClassId == entity.ClassId &&
                e.Status == (int)EnrollmentStatus.Approved);
            if (!canAccess)
                return Forbid();
        }
        else
        {
            return Forbid();
        }

        var fullPath = ResolveStoredFilePath(entity.FilePath);
        if (!System.IO.File.Exists(fullPath))
            return NotFound(new { message = "File tài liệu không tồn tại trên máy chủ." });

        var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        return File(bytes, "application/octet-stream", entity.OriginalFileName);
    }

    private static LearningMaterialDto MapDto(LearningMaterial m) => new()
    {
        Id = m.Id,
        ClassId = m.ClassId,
        ClassName = m.Class.Name,
        TeacherId = m.TeacherId,
        TeacherName = m.Teacher.FullName,
        Title = m.Title,
        Description = m.Description,
        FilePath = m.FilePath,
        OriginalFileName = m.OriginalFileName,
        FileSize = m.FileSize,
        CreatedAt = m.CreatedAt
    };

    private string ResolveStoredFilePath(string filePath)
    {
        var clean = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

        var webRoot = _env.WebRootPath;
        if (!string.IsNullOrWhiteSpace(webRoot))
        {
            var candidate = Path.Combine(webRoot, clean);
            if (System.IO.File.Exists(candidate))
                return candidate;
        }

        var contentRootCandidate = Path.Combine(_env.ContentRootPath, "wwwroot", clean);
        if (System.IO.File.Exists(contentRootCandidate))
            return contentRootCandidate;

        return Path.Combine(AppContext.BaseDirectory, "wwwroot", clean);
    }
}

public class CreateLearningMaterialForm
{
    public int ClassId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile? File { get; set; }
}
