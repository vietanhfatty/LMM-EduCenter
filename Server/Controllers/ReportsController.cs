using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Attendance;
using Server.DTOs.Report;
using Server.Models;

namespace Server.Controllers;

[Route("api/reports")]
[ApiController]
[Authorize(Roles = "Staff")]
public class ReportsController : ControllerBase
{
    private readonly LMMDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public ReportsController(LMMDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
        var students = await _userManager.GetUsersInRoleAsync("Student");

        var dto = new DashboardDto
        {
            TotalCourses = await _db.Courses.CountAsync(c => c.IsActive),
            TotalClasses = await _db.Classes.CountAsync(),
            TotalTeachers = teachers.Count(t => t.IsActive),
            TotalStudents = students.Count(s => s.IsActive),
            ActiveClasses = await _db.Classes.CountAsync(c => c.Status == (int)ClassStatus.InProgress),
            PendingEnrollments = await _db.Enrollments.CountAsync(e => e.Status == (int)EnrollmentStatus.Pending),
            MonthlyRevenue = await _db.Payments
                .Where(p => p.PaymentDate >= startOfMonth && p.Status == (int)PaymentStatus.Completed)
                .SumAsync(p => p.Amount)
        };

        return Ok(dto);
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueReportDto>> GetRevenue(
        [FromQuery] int? year = null, [FromQuery] int? month = null)
    {
        var now = DateTime.UtcNow;
        year ??= now.Year;

        var query = _db.Payments
            .Where(p => p.Status == (int)PaymentStatus.Completed && p.PaymentDate.Year == year);

        if (month.HasValue)
            query = query.Where(p => p.PaymentDate.Month == month.Value);

        var payments = await query.ToListAsync();

        var monthlyData = payments
            .GroupBy(p => p.PaymentDate.Month)
            .Select(g => new MonthlyRevenueDto { Month = g.Key, Total = g.Sum(p => p.Amount) })
            .OrderBy(m => m.Month)
            .ToList();

        return Ok(new RevenueReportDto
        {
            Year = year.Value,
            TotalRevenue = payments.Sum(p => p.Amount),
            MonthlyData = monthlyData
        });
    }

    [HttpGet("enrollment-stats")]
    public async Task<ActionResult<object>> GetEnrollmentStats()
    {
        var now = DateTime.UtcNow;
        var monthlyStats = await _db.Enrollments
            .Where(e => e.EnrollDate.Year == now.Year)
            .GroupBy(e => e.EnrollDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                Total = g.Count(),
                Approved = g.Count(e => e.Status == (int)EnrollmentStatus.Approved),
                Pending = g.Count(e => e.Status == (int)EnrollmentStatus.Pending),
                Rejected = g.Count(e => e.Status == (int)EnrollmentStatus.Rejected)
            })
            .OrderBy(x => x.Month)
            .ToListAsync();

        return Ok(monthlyStats);
    }
}
