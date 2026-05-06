using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentScheduleController : BaseController
{
    private readonly IEnrollmentApiClient _enrollmentApi;
    private const int DefaultPageSize = 10;

    public StudentScheduleController(IEnrollmentApiClient enrollmentApi) => _enrollmentApi = enrollmentApi;

    public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.GetMyAsync(token);
        var approved = result.Data?
            .Where(e => e.Status == 1)
            .ToList() ?? new List<EnrollmentDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            approved = approved.Where(e =>
                e.ClassName.ToLower().Contains(normalized) ||
                e.CourseName.ToLower().Contains(normalized) ||
                e.TeacherName.ToLower().Contains(normalized)).ToList();
        }

        var totalItems = approved.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = approved
            .OrderBy(e => e.ClassName)
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
}
