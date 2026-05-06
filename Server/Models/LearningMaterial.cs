namespace Server.Models;

public class LearningMaterial
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string TeacherId { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;
    public virtual AppUser Teacher { get; set; } = null!;
}
