using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffReportController : BaseController
{
    private readonly IPaymentApiClient _paymentApi;
    private readonly IClassApiClient _classApi;
    private readonly IAttendanceApiClient _attendanceApi;

    public StaffReportController(IPaymentApiClient paymentApi, IClassApiClient classApi, IAttendanceApiClient attendanceApi)
    {
        _paymentApi = paymentApi;
        _classApi = classApi;
        _attendanceApi = attendanceApi;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Revenue(int? year = null)
    {
        var token = GetToken()!;
        var result = await _paymentApi.GetRevenueAsync(token, year);
        return View(result.Data);
    }

    public async Task<IActionResult> Attendance(int? classId = null)
    {
        var token = GetToken()!;
        var classResult = await _classApi.GetAllAsync(token);
        ViewBag.Classes = classResult.Data ?? new List<ClassDto>();
        ViewBag.SelectedClassId = classId;

        if (classId.HasValue)
        {
            var result = await _attendanceApi.GetReportAsync(classId.Value, token);
            return View(result.Data ?? new List<AttendanceReportDto>());
        }

        return View(new List<AttendanceReportDto>());
    }
}
