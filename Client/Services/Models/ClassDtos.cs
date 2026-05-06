namespace Client.Services.Models;

public class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentStudents { get; set; }
    public int Status { get; set; }
    public string StatusText => Status switch
    {
        0 => "Sắp khai giảng",
        1 => "Đang diễn ra",
        2 => "Đã hoàn thành",
        3 => "Đã hủy",
        _ => "Không rõ"
    };
    public string StatusBadge => Status switch
    {
        0 => "bg-info",
        1 => "bg-success",
        2 => "bg-secondary",
        3 => "bg-danger",
        _ => "bg-secondary"
    };
    public List<ClassScheduleDto> Schedules { get; set; } = new();
}

public class ClassScheduleDto
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekText => DayOfWeek switch
    {
        0 => "Chủ nhật", 1 => "Thứ 2", 2 => "Thứ 3", 3 => "Thứ 4",
        4 => "Thứ 5", 5 => "Thứ 6", 6 => "Thứ 7", _ => ""
    };
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(7);
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(3);
    public int MaxStudents { get; set; } = 30;
    public int Status { get; set; }
    public List<CreateScheduleRequest> Schedules { get; set; } = new();
}

public class UpdateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int Status { get; set; }
    public List<CreateScheduleRequest> Schedules { get; set; } = new();
}

public class CreateScheduleRequest
{
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = "08:30";
    public string EndTime { get; set; } = "10:30";
}
