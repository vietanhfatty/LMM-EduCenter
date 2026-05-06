using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public int ClassId { get; set; }

    public decimal Amount { get; set; }

    public int Method { get; set; }

    public int Status { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Note { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual AppUser Student { get; set; } = null!;
}
