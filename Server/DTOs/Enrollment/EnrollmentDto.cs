namespace Server.DTOs.Enrollment;

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
    public List<EnrollmentScheduleDto> Schedules { get; set; } = new();
}

public class EnrollmentScheduleDto
{
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

public class CreateEnrollmentDto
{
    public int ClassId { get; set; }
}
