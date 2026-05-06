using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Notification;
using Server.Hubs;
using Server.Models;

namespace Server.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly LMMDbContext _db;
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationsController(LMMDbContext db, IHubContext<NotificationHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        var query = _db.Notifications
            .Include(n => n.Sender)
            .Include(n => n.NotificationReads)
            .AsQueryable();

        query = query.Where(n =>
            n.TargetType == (int)NotificationTargetType.All
            || (n.TargetType == (int)NotificationTargetType.Role && n.TargetId == userRole)
            || (n.TargetType == (int)NotificationTargetType.User && n.TargetId == userId)
            || n.SenderId == userId);

        var list = await query
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                SenderName = n.Sender.FullName,
                TargetType = n.TargetType,
                TargetId = n.TargetId,
                CreatedAt = n.CreatedAt,
                IsRead = n.NotificationReads.Any(nr => nr.UserId == userId)
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var n = await _db.Notifications
            .Include(x => x.Sender)
            .Include(x => x.NotificationReads)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (n == null) return NotFound();

        return Ok(new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            SenderName = n.Sender.FullName,
            TargetType = n.TargetType,
            TargetId = n.TargetId,
            CreatedAt = n.CreatedAt,
            IsRead = n.NotificationReads.Any(nr => nr.UserId == userId)
        });
    }

    [HttpPost]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var entity = new Notification
        {
            Title = dto.Title,
            Content = dto.Content,
            SenderId = userId!,
            TargetType = dto.TargetType,
            TargetId = dto.TargetId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Notifications.Add(entity);
        await _db.SaveChangesAsync();

        var target = dto.TargetType switch
        {
            (int)NotificationTargetType.Role => $"role_{dto.TargetId}",
            (int)NotificationTargetType.User => $"user_{dto.TargetId}",
            _ => null
        };

        var payload = new { entity.Id, entity.Title, entity.CreatedAt };

        if (target != null)
            await _hub.Clients.Group(target).SendAsync("ReceiveNotification", payload);
        else
            await _hub.Clients.All.SendAsync("ReceiveNotification", payload);

        var created = await _db.Notifications
            .Include(n => n.Sender)
            .FirstAsync(n => n.Id == entity.Id);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new NotificationDto
        {
            Id = created.Id,
            Title = created.Title,
            Content = created.Content,
            SenderName = created.Sender.FullName,
            TargetType = created.TargetType,
            TargetId = created.TargetId,
            CreatedAt = created.CreatedAt,
            IsRead = false
        });
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exists = await _db.NotificationReads.AnyAsync(nr =>
            nr.NotificationId == id && nr.UserId == userId);

        if (!exists)
        {
            _db.NotificationReads.Add(new NotificationRead
            {
                NotificationId = id,
                UserId = userId!,
                ReadAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Đã đọc." });
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        var count = await _db.Notifications
            .Include(n => n.NotificationReads)
            .Where(n =>
                n.TargetType == (int)NotificationTargetType.All
                || (n.TargetType == (int)NotificationTargetType.Role && n.TargetId == userRole)
                || (n.TargetType == (int)NotificationTargetType.User && n.TargetId == userId))
            .CountAsync(n => !n.NotificationReads.Any(nr => nr.UserId == userId));

        return Ok(count);
    }
}
