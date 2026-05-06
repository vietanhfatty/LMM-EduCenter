using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.User;
using Server.Models;

namespace Server.Controllers;

[Route("api/students")]
[ApiController]
[Authorize(Roles = "Staff")]
public class StudentsController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly LMMDbContext _db;

    public StudentsController(UserManager<AppUser> userManager, LMMDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var students = await _userManager.GetUsersInRoleAsync("Student");
        var list = students.OrderByDescending(u => u.CreatedAt).Select(MapUser).ToList();
        foreach (var dto in list) dto.Roles = new List<string> { "Student" };
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Student")) return NotFound();

        var dto = MapUser(user);
        dto.Roles = roles.ToList();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return BadRequest(new { message = "Email đã được sử dụng." });

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            Phone = dto.Phone,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        await _userManager.AddToRoleAsync(user, "Student");

        var resp = MapUser(user);
        resp.Roles = new List<string> { "Student" };
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, resp);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.FullName = dto.FullName;
        user.Phone = dto.Phone;
        user.Address = dto.Address;
        user.DateOfBirth = dto.DateOfBirth;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        var roles = await _userManager.GetRolesAsync(user);
        var resp = MapUser(user);
        resp.Roles = roles.ToList();
        return Ok(resp);
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> Toggle(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        return Ok(new { user.IsActive });
    }

    [HttpGet("{id}/enrollments")]
    public async Task<IActionResult> GetEnrollments(string id)
    {
        var enrollments = await _db.Enrollments
            .Where(e => e.StudentId == id)
            .Include(e => e.Class).ThenInclude(c => c.Course)
            .OrderByDescending(e => e.EnrollDate)
            .Select(e => new
            {
                e.Id,
                e.ClassId,
                ClassName = e.Class.Name,
                CourseName = e.Class.Course.Name,
                e.EnrollDate,
                e.Status
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    private static UserDto MapUser(AppUser u) => new()
    {
        Id = u.Id,
        Email = u.Email ?? string.Empty,
        FullName = u.FullName,
        Phone = u.Phone,
        Avatar = u.Avatar,
        DateOfBirth = u.DateOfBirth,
        Address = u.Address,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
