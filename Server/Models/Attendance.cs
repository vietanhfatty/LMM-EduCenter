using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Attendance
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public string StudentId { get; set; } = null!;

    public DateTime Date { get; set; }

    public int Status { get; set; }

    public string? Note { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual AppUser Student { get; set; } = null!;
}
