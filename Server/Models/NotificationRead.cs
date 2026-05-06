using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class NotificationRead
{
    public int Id { get; set; }

    public int NotificationId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime ReadAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
