namespace Server.DTOs.Notification;

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
}

public class CreateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int TargetType { get; set; }
    public string? TargetId { get; set; }
}
