using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffCourseController : BaseController
{
    private readonly ICourseApiClient _api;
    private const int DefaultPageSize = 10;

    public StaffCourseController(ICourseApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword, bool? isActive, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var result = await _api.GetAllAsync(token);
        var courses = result.Data ?? new List<CourseDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            courses = courses
                .Where(c =>
                    c.Name.ToLower().Contains(normalized) ||
                    (c.Description != null && c.Description.ToLower().Contains(normalized)))
                .ToList();
        }

        if (isActive.HasValue)
            courses = courses.Where(c => c.IsActive == isActive.Value).ToList();

        var totalItems = courses.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = courses
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Keyword = keyword;
        ViewBag.IsActive = isActive;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    public IActionResult Create() => View(new CreateCourseRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.CreateAsync(GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Tạo khóa học thất bại.");
            return View(model);
        }

        TempData["Success"] = "Tạo khóa học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _api.GetByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy khóa học.";
            return RedirectToAction(nameof(Index));
        }

        var model = new UpdateCourseRequest
        {
            Name = result.Data.Name,
            Description = result.Data.Description,
            Fee = result.Data.Fee,
            DurationInHours = result.Data.DurationInHours,
            IsActive = result.Data.IsActive
        };

        ViewBag.CourseId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCourseRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CourseId = id;
            return View(model);
        }

        var result = await _api.UpdateAsync(id, GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Cập nhật thất bại.");
            ViewBag.CourseId = id;
            return View(model);
        }

        TempData["Success"] = "Cập nhật khóa học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _api.GetByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy khóa học.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _api.DeleteAsync(id, GetToken()!);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Xóa thất bại.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Xóa khóa học thành công.";
        return RedirectToAction(nameof(Index));
    }
}
