namespace Client.Services.Models;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public int TargetType { get; set; }
    public string? TargetId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }

    public string TargetText => TargetType switch
    {
        0 => "Tất cả",
        1 => $"Role: {TargetId}",
        2 => $"Lớp: {TargetId}",
        3 => "Cá nhân",
        _ => ""
    };
}

public class CreateNotificationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int TargetType { get; set; }
    public string? TargetId { get; set; }
}
