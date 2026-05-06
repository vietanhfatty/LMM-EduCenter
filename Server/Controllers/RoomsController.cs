using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Room;
using Server.Models;

namespace Server.Controllers;

[Route("api/rooms")]
[ApiController]
[Authorize(Roles = "Staff")]
public class RoomsController : ControllerBase
{
    private readonly LMMDbContext _db;

    public RoomsController(LMMDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
    {
        var list = await _db.Rooms
            .Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Location = r.Location,
                IsActive = r.IsActive,
                ClassCount = r.Classes.Count
            })
            .OrderBy(r => r.Name)
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var r = await _db.Rooms
            .Where(x => x.Id == id)
            .Select(x => new RoomDto
            {
                Id = x.Id,
                Name = x.Name,
                Capacity = x.Capacity,
                Location = x.Location,
                IsActive = x.IsActive,
                ClassCount = x.Classes.Count
            })
            .FirstOrDefaultAsync();

        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
    {
        var entity = new Room
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            Location = dto.Location,
            IsActive = dto.IsActive
        };

        _db.Rooms.Add(entity);
        await _db.SaveChangesAsync();

        var result = new RoomDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Capacity = entity.Capacity,
            Location = entity.Location,
            IsActive = entity.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        var entity = await _db.Rooms.FindAsync(id);
        if (entity == null) return NotFound();

        entity.Name = dto.Name;
        entity.Capacity = dto.Capacity;
        entity.Location = dto.Location;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        var result = new RoomDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Capacity = entity.Capacity,
            Location = entity.Location,
            IsActive = entity.IsActive
        };

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Rooms.FindAsync(id);
        if (entity == null) return NotFound();

        var hasClasses = await _db.Classes.AnyAsync(c => c.RoomId == id);
        if (hasClasses)
            return BadRequest(new { message = "Không thể xóa phòng học đang được sử dụng." });

        _db.Rooms.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
