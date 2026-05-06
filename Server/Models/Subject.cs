using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;
}
