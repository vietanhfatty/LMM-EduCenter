using Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffDashboardController : BaseController
{
    private readonly IReportApiClient _reportApi;

    public StaffDashboardController(IReportApiClient reportApi) => _reportApi = reportApi;

    public async Task<IActionResult> Index()
    {
        var result = await _reportApi.GetDashboardAsync(GetToken()!);
        return View(result.Data);
    }
}
