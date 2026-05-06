namespace Client.Services.Models;

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

    public string StatusText => Status switch
    {
        0 => "Có mặt",
        1 => "Vắng",
        2 => "Đi muộn",
        3 => "Có phép",
        _ => ""
    };

    public string StatusBadge => Status switch
    {
        0 => "bg-success",
        1 => "bg-danger",
        2 => "bg-warning text-dark",
        3 => "bg-info",
        _ => "bg-secondary"
    };
}

public class BatchAttendanceRequest
{
    public int ClassId { get; set; }
    public DateTime Date { get; set; }
    public List<AttendanceItemRequest> Items { get; set; } = new();
}

public class AttendanceItemRequest
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
