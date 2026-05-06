using Client.Services.Models;

namespace Client.Services;

public class EnrollmentApiClient : BaseApiClient, IEnrollmentApiClient
{
    public EnrollmentApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<EnrollmentDto>>> GetAllAsync(string token, int? status = null)
    {
        var path = "api/enrollments";
        if (status.HasValue) path += $"?status={status.Value}";
        return GetAsync<List<EnrollmentDto>>(path, token);
    }

    public Task<ApiResult<List<EnrollmentDto>>> GetMyAsync(string token)
        => GetAsync<List<EnrollmentDto>>("api/enrollments/my", token);

    public Task<ApiResult<EnrollmentDto>> CreateAsync(string token, CreateEnrollmentRequest request)
        => PostAsync<EnrollmentDto>("api/enrollments", request, token);

    public Task<ApiResult<bool>> ApproveAsync(int id, string token)
        => PostNoContentAsync($"api/enrollments/{id}/approve", new { }, token);

    public Task<ApiResult<bool>> RejectAsync(int id, string token)
        => PostNoContentAsync($"api/enrollments/{id}/reject", new { }, token);

    public Task<ApiResult<bool>> CancelAsync(int id, string token)
        => PostNoContentAsync($"api/enrollments/{id}/cancel", new { }, token);
}
