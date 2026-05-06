using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffEnrollmentController : BaseController
{
    private readonly IEnrollmentApiClient _enrollmentApi;
    private const int DefaultPageSize = 10;

    public StaffEnrollmentController(IEnrollmentApiClient enrollmentApi) => _enrollmentApi = enrollmentApi;

    public async Task<IActionResult> Index(int? status = null, string? keyword = null, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.GetAllAsync(token, status);
        var enrollments = result.Data ?? new List<EnrollmentDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            enrollments = enrollments.Where(e =>
                e.StudentName.ToLower().Contains(normalized) ||
                e.StudentEmail.ToLower().Contains(normalized) ||
                e.ClassName.ToLower().Contains(normalized) ||
                e.CourseName.ToLower().Contains(normalized)).ToList();
        }

        var totalItems = enrollments.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = enrollments
            .OrderByDescending(e => e.EnrollDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentStatus = status;
        ViewBag.Keyword = keyword;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.GetAllAsync(token);
        var enrollment = result.Data?.FirstOrDefault(e => e.Id == id);

        if (enrollment == null)
        {
            TempData["Error"] = "Không tìm thấy đăng ký.";
            return RedirectToAction(nameof(Index));
        }

        return View(enrollment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.ApproveAsync(id, token);

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Duyệt thất bại.";
        }
        else
        {
            TempData["Success"] = "Duyệt đăng ký thành công.";
        }

        return RedirectToAction(nameof(Index), new { status = 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.RejectAsync(id, token);

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Từ chối thất bại.";
        }
        else
        {
            TempData["Success"] = "Đã từ chối đăng ký.";
        }

        return RedirectToAction(nameof(Index), new { status = 0 });
    }
}
