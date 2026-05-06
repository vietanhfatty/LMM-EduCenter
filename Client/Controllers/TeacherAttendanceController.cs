using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Teacher")]
public class TeacherAttendanceController : BaseController
{
    private readonly IClassApiClient _classApi;
    private readonly IAttendanceApiClient _attendanceApi;

    public TeacherAttendanceController(IClassApiClient classApi, IAttendanceApiClient attendanceApi)
    {
        _classApi = classApi;
        _attendanceApi = attendanceApi;
    }

    public IActionResult Index()
    {
        var selectedDate = ResolveSelectedDate(Request.Query["date"]);
        return RedirectToAction("Index", "TeacherSchedule", new { date = selectedDate.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> Take(int id, DateTime? date = null)
    {
        var token = GetToken()!;
        var classResult = await _classApi.GetByIdAsync(id, token);
        if (!classResult.Success || classResult.Data == null)
        {
            TempData["Error"] = "Không tìm thấy lớp học.";
            return RedirectToAction("Index", "TeacherSchedule");
        }

        var selectedDate = (date ?? DateTime.Today).Date;
        var attendanceResult = await _attendanceApi.GetByClassAsync(id, token, selectedDate);
        ViewBag.ClassInfo = classResult.Data;
        ViewBag.ExistingAttendance = attendanceResult.Data ?? new List<AttendanceDto>();
        ViewBag.Date = selectedDate;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Take(int id, BatchAttendanceRequest model)
    {
        var token = GetToken()!;
        model.ClassId = id;

        var result = await _attendanceApi.BatchCreateAsync(token, model);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Điểm danh thất bại.";
            return RedirectToAction(nameof(Take), new { id, date = model.Date.ToString("yyyy-MM-dd") });
        }

        TempData["Success"] = "Điểm danh thành công.";
        return RedirectToAction(nameof(Take), new { id, date = model.Date.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> History(int id)
    {
        var token = GetToken()!;
        var classResult = await _classApi.GetByIdAsync(id, token);
        var reportResult = await _attendanceApi.GetReportAsync(id, token);

        ViewBag.ClassInfo = classResult.Data;
        return View(reportResult.Data ?? new List<AttendanceReportDto>());
    }

    private static DateTime ResolveSelectedDate(string? dateQuery)
    {
        if (!string.IsNullOrWhiteSpace(dateQuery) && DateTime.TryParse(dateQuery, out var parsed))
            return parsed.Date;

        return DateTime.Today;
    }

}
