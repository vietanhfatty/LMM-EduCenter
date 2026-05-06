using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Teacher")]
public class TeacherDashboardController : BaseController
{
    public IActionResult Index() => View();
}
