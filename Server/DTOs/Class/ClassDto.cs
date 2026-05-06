namespace Server.DTOs.Class;

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
        0 => "Sap khai giang",
        1 => "Dang dien ra",
        2 => "Da hoan thanh",
        3 => "Da huy",
        _ => "Khong ro"
    };
    public List<ClassScheduleDto> Schedules { get; set; } = new();
}

public class ClassScheduleDto
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekText => DayOfWeek switch
    {
        0 => "Chu nhat", 1 => "Thu 2", 2 => "Thu 3", 3 => "Thu 4",
        4 => "Thu 5", 5 => "Thu 6", 6 => "Thu 7", _ => ""
    };
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int Status { get; set; }
    public List<CreateScheduleDto> Schedules { get; set; } = new();
}

public class UpdateClassDto
{
    public string Name { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxStudents { get; set; }
    public int Status { get; set; }
    public List<CreateScheduleDto> Schedules { get; set; } = new();
}

public class CreateScheduleDto
{
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
