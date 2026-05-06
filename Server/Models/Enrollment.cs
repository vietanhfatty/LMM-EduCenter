using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Enrollment
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public int ClassId { get; set; }

    public DateTime EnrollDate { get; set; }

    public int Status { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual AppUser Student { get; set; } = null!;
}
