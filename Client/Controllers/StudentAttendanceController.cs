using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentAttendanceController : BaseController
{
    private readonly IAttendanceApiClient _attendanceApi;
    private readonly IEnrollmentApiClient _enrollmentApi;
    private const int DefaultPageSize = 10;

    public StudentAttendanceController(IAttendanceApiClient attendanceApi, IEnrollmentApiClient enrollmentApi)
    {
        _attendanceApi = attendanceApi;
        _enrollmentApi = enrollmentApi;
    }

    public async Task<IActionResult> Index(int? classId = null, string? keyword = null, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;

        var enrollResult = await _enrollmentApi.GetMyAsync(token);
        var approvedClasses = enrollResult.Data?
            .Where(e => e.Status == 1)
            .ToList() ?? new List<EnrollmentDto>();
        ViewBag.Classes = approvedClasses;
        ViewBag.SelectedClassId = classId;

        if (classId.HasValue)
        {
            var result = await _attendanceApi.GetMyAsync(token, classId);
            var attendances = result.Data ?? new List<AttendanceDto>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalized = keyword.Trim().ToLower();
                attendances = attendances.Where(a =>
                    a.Date.ToString("dd/MM/yyyy").Contains(normalized) ||
                    a.StatusText.ToLower().Contains(normalized) ||
                    (a.Note != null && a.Note.ToLower().Contains(normalized))).ToList();
            }

            var totalItems = attendances.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var pagedData = attendances
                .OrderByDescending(a => a.Date)
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

        ViewBag.Keyword = keyword;
        ViewBag.Page = 1;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = 0;
        ViewBag.TotalPages = 1;
        return View(new List<AttendanceDto>());
    }
}
