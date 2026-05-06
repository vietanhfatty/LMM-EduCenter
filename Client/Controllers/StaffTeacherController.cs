using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffTeacherController : BaseController
{
    private readonly IUserApiClient _api;
    private const int DefaultPageSize = 10;

    public StaffTeacherController(IUserApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword, bool? isActive, int page = 1, int pageSize = DefaultPageSize)
    {
        var result = await _api.GetTeachersAsync(GetToken()!);
        var teachers = result.Data ?? new List<UserDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            teachers = teachers.Where(t =>
                t.FullName.ToLower().Contains(normalized) ||
                t.Email.ToLower().Contains(normalized) ||
                (t.Phone != null && t.Phone.ToLower().Contains(normalized))).ToList();
        }

        if (isActive.HasValue)
            teachers = teachers.Where(t => t.IsActive == isActive.Value).ToList();

        var totalItems = teachers.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = teachers
            .OrderBy(t => t.FullName)
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

    public IActionResult Create() => View(new CreateUserRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.CreateTeacherAsync(GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Tạo giáo viên thất bại.");
            return View(model);
        }

        TempData["Success"] = "Tạo tài khoản giáo viên thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var result = await _api.GetTeacherByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy giáo viên.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.UserId = id;
        ViewBag.Email = result.Data.Email;
        return View(new UpdateUserRequest
        {
            FullName = result.Data.FullName,
            Phone = result.Data.Phone,
            Address = result.Data.Address,
            DateOfBirth = result.Data.DateOfBirth
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UpdateUserRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserId = id;
            return View(model);
        }

        var result = await _api.UpdateTeacherAsync(id, GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Cập nhật thất bại.");
            ViewBag.UserId = id;
            return View(model);
        }

        TempData["Success"] = "Cập nhật thông tin giáo viên thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        var result = await _api.GetTeacherByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy giáo viên.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(string id)
    {
        await _api.ToggleTeacherAsync(id, GetToken()!);
        TempData["Success"] = "Chuyển trạng thái thành công.";
        return RedirectToAction(nameof(Index));
    }
}
