using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Grade
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public int ClassId { get; set; }

    public int Type { get; set; }

    public string? Description { get; set; }

    public double Score { get; set; }

    public double Weight { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual AppUser Student { get; set; } = null!;
}
