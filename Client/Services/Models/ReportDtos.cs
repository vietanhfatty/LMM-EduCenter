namespace Client.Services.Models;

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
