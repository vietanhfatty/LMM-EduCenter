namespace Server.DTOs.Grade;

public class GradeDto
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Type { get; set; }
    public string? Description { get; set; }
    public double Score { get; set; }
    public double Weight { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BatchGradeDto
{
    public int ClassId { get; set; }
    public int Type { get; set; }
    public string? Description { get; set; }
    public double Weight { get; set; }
    public List<GradeItemDto> Items { get; set; } = new();
}

public class GradeItemDto
{
    public string StudentId { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class StudentGradeSummaryDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public List<GradeDto> Grades { get; set; } = new();
    public double? WeightedAverage { get; set; }
}
