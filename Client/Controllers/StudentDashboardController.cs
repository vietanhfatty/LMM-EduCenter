using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentDashboardController : BaseController
{
    public IActionResult Index() => View();
}
