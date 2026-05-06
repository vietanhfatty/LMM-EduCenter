using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentCourseController : BaseController
{
    private readonly ICourseApiClient _courseApi;
    private readonly IClassApiClient _classApi;
    private const int DefaultPageSize = 9;

    public StudentCourseController(ICourseApiClient courseApi, IClassApiClient classApi)
    {
        _courseApi = courseApi;
        _classApi = classApi;
    }

    public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var result = await _courseApi.GetAllAsync(token);
        var courses = result.Data?.Where(c => c.IsActive).ToList() ?? new List<CourseDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            courses = courses.Where(c =>
                c.Name.ToLower().Contains(normalized) ||
                (c.Description != null && c.Description.ToLower().Contains(normalized))).ToList();
        }

        var totalItems = courses.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = courses
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

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
        var courseResult = await _courseApi.GetByIdAsync(id, token);
        if (!courseResult.Success || courseResult.Data == null)
        {
            TempData["Error"] = "Không tìm thấy khóa học.";
            return RedirectToAction(nameof(Index));
        }

        var classResult = await _classApi.GetAllAsync(token);
        var availableClasses = classResult.Data?
            .Where(c => c.CourseId == id && (c.Status == 0 || c.Status == 1) && c.CurrentStudents < c.MaxStudents)
            .ToList() ?? new List<ClassDto>();

        ViewBag.AvailableClasses = availableClasses;
        return View(courseResult.Data);
    }
}
