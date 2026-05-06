using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentGradeController : BaseController
{
    private readonly IGradeApiClient _gradeApi;

    public StudentGradeController(IGradeApiClient gradeApi) => _gradeApi = gradeApi;

    public async Task<IActionResult> Index(string? keyword)
    {
        var token = GetToken()!;
        var result = await _gradeApi.GetMyAsync(token);
        var grades = result.Data ?? new List<StudentGradeSummaryDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            grades = grades.Where(g =>
                g.ClassName.ToLower().Contains(normalized) ||
                g.CourseName.ToLower().Contains(normalized)).ToList();
        }

        ViewBag.Keyword = keyword;
        return View(grades);
    }
}
