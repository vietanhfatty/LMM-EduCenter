using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Teacher")]
public class TeacherGradeController : BaseController
{
    private readonly IClassApiClient _classApi;
    private readonly IGradeApiClient _gradeApi;
    private const int DefaultPageSize = 10;

    public TeacherGradeController(IClassApiClient classApi, IGradeApiClient gradeApi)
    {
        _classApi = classApi;
        _gradeApi = gradeApi;
    }

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
                c.CourseName.ToLower().Contains(normalized)).ToList();
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

    public async Task<IActionResult> ClassGrades(int id)
    {
        var token = GetToken()!;
        var classResult = await _classApi.GetByIdAsync(id, token);
        var gradeResult = await _gradeApi.GetByClassAsync(id, token);

        ViewBag.ClassInfo = classResult.Data;
        return View(gradeResult.Data ?? new List<GradeDto>());
    }

    public async Task<IActionResult> Enter(int id)
    {
        var token = GetToken()!;
        var classResult = await _classApi.GetByIdAsync(id, token);
        if (!classResult.Success || classResult.Data == null)
        {
            TempData["Error"] = "Không tìm thấy lớp học.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.ClassInfo = classResult.Data;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enter(int id, BatchGradeRequest model)
    {
        var token = GetToken()!;
        model.ClassId = id;

        var result = await _gradeApi.BatchCreateAsync(token, model);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Lưu điểm thất bại.";
            return RedirectToAction(nameof(Enter), new { id });
        }

        TempData["Success"] = "Lưu điểm thành công.";
        return RedirectToAction(nameof(ClassGrades), new { id });
    }
}
