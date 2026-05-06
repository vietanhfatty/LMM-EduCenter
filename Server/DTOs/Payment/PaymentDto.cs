namespace Server.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public int Status { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Note { get; set; }
}

public class CreatePaymentDto
{
    public string StudentId { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public string? Note { get; set; }
}

public class CreateBankTransferPaymentDto
{
    public int ClassId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}

public class DebtDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public decimal CourseFee { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Remaining { get; set; }
}
