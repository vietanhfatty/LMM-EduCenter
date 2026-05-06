using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffNotificationController : BaseController
{
    private readonly INotificationApiClient _notiApi;
    private const int DefaultPageSize = 10;

    public StaffNotificationController(INotificationApiClient notiApi) => _notiApi = notiApi;

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

    public IActionResult Create() => View(new CreateNotificationRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNotificationRequest model)
    {
        var token = GetToken()!;
        var result = await _notiApi.CreateAsync(token, model);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Gửi thông báo thất bại.";
            return View(model);
        }

        TempData["Success"] = "Gửi thông báo thành công.";
        return RedirectToAction(nameof(Index));
    }
}
