using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffRoomController : BaseController
{
    private readonly IRoomApiClient _api;
    private const int DefaultPageSize = 10;

    public StaffRoomController(IRoomApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword, bool? isActive, int page = 1, int pageSize = DefaultPageSize)
    {
        var result = await _api.GetAllAsync(GetToken()!);
        var rooms = result.Data ?? new List<RoomDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            rooms = rooms.Where(r =>
                r.Name.ToLower().Contains(normalized) ||
                (r.Location != null && r.Location.ToLower().Contains(normalized))).ToList();
        }

        if (isActive.HasValue)
            rooms = rooms.Where(r => r.IsActive == isActive.Value).ToList();

        var totalItems = rooms.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = rooms
            .OrderBy(r => r.Name)
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

    public IActionResult Create() => View(new CreateRoomRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.CreateAsync(GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Tạo phòng học thất bại.");
            return View(model);
        }

        TempData["Success"] = "Tạo phòng học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _api.GetByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy phòng học.";
            return RedirectToAction(nameof(Index));
        }

        var model = new UpdateRoomRequest
        {
            Name = result.Data.Name,
            Capacity = result.Data.Capacity,
            Location = result.Data.Location,
            IsActive = result.Data.IsActive
        };

        ViewBag.RoomId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateRoomRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.RoomId = id;
            return View(model);
        }

        var result = await _api.UpdateAsync(id, GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Cập nhật thất bại.");
            ViewBag.RoomId = id;
            return View(model);
        }

        TempData["Success"] = "Cập nhật phòng học thành công.";
        return RedirectToAction(nameof(Index));
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

        TempData["Success"] = "Xóa phòng học thành công.";
        return RedirectToAction(nameof(Index));
    }
}
