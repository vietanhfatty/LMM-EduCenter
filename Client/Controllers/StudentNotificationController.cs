using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentNotificationController : BaseController
{
    private readonly INotificationApiClient _notiApi;
    private const int DefaultPageSize = 10;

    public StudentNotificationController(INotificationApiClient notiApi) => _notiApi = notiApi;

    public async Task<IActionResult> Index(string? keyword, bool? isRead, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var result = await _notiApi.GetAllAsync(token);
        var notifications = result.Data ?? new List<NotificationDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            notifications = notifications.Where(n =>
                n.Title.ToLower().Contains(normalized) ||
                n.Content.ToLower().Contains(normalized) ||
                n.SenderName.ToLower().Contains(normalized)).ToList();
        }

        if (isRead.HasValue)
            notifications = notifications.Where(n => n.IsRead == isRead.Value).ToList();

        var totalItems = notifications.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = notifications
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Keyword = keyword;
        ViewBag.IsRead = isRead;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = GetToken()!;
        await _notiApi.MarkReadAsync(id, token);
        var result = await _notiApi.GetByIdAsync(id, token);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy thông báo.";
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }
}
