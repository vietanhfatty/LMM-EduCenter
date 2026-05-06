namespace Client.Services.Models;

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

    public string MethodText => Method switch
    {
        0 => "Tien mat",
        1 => "Chuyen khoan",
        2 => "The",
        _ => ""
    };

    public string StatusText => Status switch
    {
        0 => "Cho xu ly",
        1 => "Da thanh toan",
        2 => "Da hoan",
        _ => ""
    };

    public string StatusBadge => Status switch
    {
        0 => "bg-warning text-dark",
        1 => "bg-success",
        2 => "bg-info",
        _ => "bg-secondary"
    };
}

public class CreatePaymentRequest
{
    public string StudentId { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public string? Note { get; set; }
}

public class CreateBankTransferRequest
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

public class RevenueReportDto
{
    public int Year { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<MonthlyRevenueDto> MonthlyData { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public int Month { get; set; }
    public decimal Total { get; set; }
}
