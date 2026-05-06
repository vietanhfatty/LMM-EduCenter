using Client.Services.Models;

namespace Client.Services;

public interface IEnrollmentApiClient
{
    Task<ApiResult<List<EnrollmentDto>>> GetAllAsync(string token, int? status = null);
    Task<ApiResult<List<EnrollmentDto>>> GetMyAsync(string token);
    Task<ApiResult<EnrollmentDto>> CreateAsync(string token, CreateEnrollmentRequest request);
    Task<ApiResult<bool>> ApproveAsync(int id, string token);
    Task<ApiResult<bool>> RejectAsync(int id, string token);
    Task<ApiResult<bool>> CancelAsync(int id, string token);
}
