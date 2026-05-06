namespace Client.Services.Models;

public class EnrollmentDto
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public DateTime EnrollDate { get; set; }
    public int Status { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentStudents { get; set; }

    public string StatusText => Status switch
    {
        0 => "Chờ duyệt",
        1 => "Đã duyệt",
        2 => "Bị từ chối",
        3 => "Đã hủy",
        _ => "Không rõ"
    };

    public string StatusBadge => Status switch
    {
        0 => "bg-warning text-dark",
        1 => "bg-success",
        2 => "bg-danger",
        3 => "bg-secondary",
        _ => "bg-secondary"
    };

    public List<EnrollmentScheduleDto> Schedules { get; set; } = new();
}

public class EnrollmentScheduleDto
{
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;

    public string DayOfWeekText => DayOfWeek switch
    {
        0 => "Chủ nhật", 1 => "Thứ 2", 2 => "Thứ 3", 3 => "Thứ 4",
        4 => "Thứ 5", 5 => "Thứ 6", 6 => "Thứ 7", _ => ""
    };
}

public class CreateEnrollmentRequest
{
    public int ClassId { get; set; }
}
