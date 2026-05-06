using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class ClassSchedule
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Class Class { get; set; } = null!;
}
