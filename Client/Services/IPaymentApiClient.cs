using Client.Services.Models;

namespace Client.Services;

public interface IPaymentApiClient
{
    Task<ApiResult<List<PaymentDto>>> GetAllAsync(string token);
    Task<ApiResult<List<PaymentDto>>> GetMyAsync(string token);
    Task<ApiResult<PaymentDto>> CreateAsync(string token, CreatePaymentRequest request);
    Task<ApiResult<PaymentDto>> CreateMyBankTransferAsync(string token, CreateBankTransferRequest request);
    Task<ApiResult<PaymentDto>> ConfirmTransferAsync(string token, int paymentId);
    Task<ApiResult<List<DebtDto>>> GetDebtsAsync(string token);
    Task<ApiResult<List<DebtDto>>> GetMyDebtsAsync(string token);
    Task<ApiResult<RevenueReportDto>> GetRevenueAsync(string token, int? year = null);
}
