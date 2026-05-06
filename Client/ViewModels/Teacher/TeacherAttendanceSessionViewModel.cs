namespace Client.ViewModels.Teacher;

public class TeacherAttendanceSessionViewModel
{
    public int ClassId { get; set; }
    public int ScheduleId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string DayOfWeekText { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int CurrentStudents { get; set; }
    public int MaxStudents { get; set; }
    public DateTime AttendanceDate { get; set; }
    public bool IsInDateRange { get; set; }
    public DateTime ClassStartDate { get; set; }
    public DateTime ClassEndDate { get; set; }
}
