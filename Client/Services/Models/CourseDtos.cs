namespace Client.Services.Models;

public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Fee { get; set; }
    public int DurationInHours { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ClassCount { get; set; }
    public int SubjectCount { get; set; }
}

public class CreateCourseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Fee { get; set; }
    public int DurationInHours { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCourseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Fee { get; set; }
    public int DurationInHours { get; set; }
    public bool IsActive { get; set; }
}
