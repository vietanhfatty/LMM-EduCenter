namespace Client.ViewModels.Teacher;

public class TeacherDailyScheduleViewModel
{
    public DateTime SelectedDate { get; set; }
    public string DayTitle { get; set; } = string.Empty;
    public List<TeacherScheduleSlotViewModel> Slots { get; set; } = new();
}

public class TeacherScheduleSlotViewModel
{
    public string SlotName { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
    public string BreakNote { get; set; } = string.Empty;
    public List<TeacherScheduleClassItemViewModel> Classes { get; set; } = new();
}

public class TeacherScheduleClassItemViewModel
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsInDateRange { get; set; }
}
