using Client.Services.Models;

namespace Client.Services;

public class PaymentApiClient : BaseApiClient, IPaymentApiClient
{
    public PaymentApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<PaymentDto>>> GetAllAsync(string token)
        => GetAsync<List<PaymentDto>>("api/payments", token);

    public Task<ApiResult<List<PaymentDto>>> GetMyAsync(string token)
        => GetAsync<List<PaymentDto>>("api/payments/my", token);

    public Task<ApiResult<PaymentDto>> CreateAsync(string token, CreatePaymentRequest request)
        => PostAsync<PaymentDto>("api/payments", request, token);

    public Task<ApiResult<PaymentDto>> CreateMyBankTransferAsync(string token, CreateBankTransferRequest request)
        => PostAsync<PaymentDto>("api/payments/my/bank-transfer", request, token);

    public Task<ApiResult<PaymentDto>> ConfirmTransferAsync(string token, int paymentId)
        => PostAsync<PaymentDto>($"api/payments/{paymentId}/confirm-transfer", new { }, token);

    public Task<ApiResult<List<DebtDto>>> GetDebtsAsync(string token)
        => GetAsync<List<DebtDto>>("api/payments/debts", token);

    public Task<ApiResult<List<DebtDto>>> GetMyDebtsAsync(string token)
        => GetAsync<List<DebtDto>>("api/payments/my/debts", token);

    public Task<ApiResult<RevenueReportDto>> GetRevenueAsync(string token, int? year = null)
    {
        var path = "api/reports/revenue";
        if (year.HasValue) path += $"?year={year}";
        return GetAsync<RevenueReportDto>(path, token);
    }
}
