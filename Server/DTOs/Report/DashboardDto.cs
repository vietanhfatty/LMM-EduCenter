namespace Server.DTOs.Report;

public class DashboardDto
{
    public int TotalCourses { get; set; }
    public int TotalClasses { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int ActiveClasses { get; set; }
    public int PendingEnrollments { get; set; }
    public decimal MonthlyRevenue { get; set; }
}

public class RevenueReportDto
{
    public int Year { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<MonthlyRevenueDto> MonthlyData { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public int Month { get; set; }
    public decimal Total { get; set; }
}
