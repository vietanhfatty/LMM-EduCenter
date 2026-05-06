namespace Server.DTOs.Attendance;

public class AttendanceDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Status { get; set; }
    public string? Note { get; set; }
}

public class BatchAttendanceDto
{
    public int ClassId { get; set; }
    public DateTime Date { get; set; }
    public List<AttendanceItemDto> Items { get; set; } = new();
}

public class AttendanceItemDto
{
    public string StudentId { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Note { get; set; }
}

public class AttendanceReportDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public double AttendanceRate { get; set; }
}
