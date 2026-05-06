using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
