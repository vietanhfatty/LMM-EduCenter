using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class TeacherReview
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public string TeacherId { get; set; } = null!;

    public int ClassId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser Student { get; set; } = null!;

    public virtual AppUser Teacher { get; set; } = null!;
}
