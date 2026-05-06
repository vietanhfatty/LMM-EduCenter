namespace Client.Services.Models;

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

    public string TypeText => Type switch
    {
        0 => "Quiz",
        1 => "Giữa kỳ",
        2 => "Cuối kỳ",
        3 => "Bài tập",
        4 => "Tham gia",
        _ => ""
    };
}

public class BatchGradeRequest
{
    public int ClassId { get; set; }
    public int Type { get; set; }
    public string? Description { get; set; }
    public double Weight { get; set; } = 1;
    public List<GradeItemRequest> Items { get; set; } = new();
}

public class GradeItemRequest
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
