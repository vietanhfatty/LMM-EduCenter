using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string SenderId { get; set; } = null!;

    public int TargetType { get; set; }

    public string? TargetId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<NotificationRead> NotificationReads { get; set; } = new List<NotificationRead>();

    public virtual AppUser Sender { get; set; } = null!;
}
