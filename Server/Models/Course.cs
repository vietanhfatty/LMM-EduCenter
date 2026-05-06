using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Fee { get; set; }

    public int DurationInHours { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
