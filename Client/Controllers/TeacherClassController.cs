using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Teacher")]
public class TeacherClassController : BaseController
{
    private readonly IClassApiClient _classApi;
    private const int DefaultPageSize = 10;

    public TeacherClassController(IClassApiClient classApi) => _classApi = classApi;

    public async Task<IActionResult> Index(string? keyword, int? status, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var userId = GetUserId()!;
        var result = await _classApi.GetAllAsync(token, userId);
        var classes = result.Data ?? new List<ClassDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            classes = classes.Where(c =>
                c.Name.ToLower().Contains(normalized) ||
                c.CourseName.ToLower().Contains(normalized) ||
                c.RoomName.ToLower().Contains(normalized)).ToList();
        }

        if (status.HasValue)
            classes = classes.Where(c => c.Status == status.Value).ToList();

        var totalItems = classes.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = classes
            .OrderByDescending(c => c.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = GetToken()!;
        var result = await _classApi.GetByIdAsync(id, token);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy lớp học.";
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }
}
