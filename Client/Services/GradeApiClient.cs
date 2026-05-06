using Client.Services.Models;

namespace Client.Services;

public class GradeApiClient : BaseApiClient, IGradeApiClient
{
    public GradeApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<GradeDto>>> GetByClassAsync(int classId, string token)
        => GetAsync<List<GradeDto>>($"api/grades?classId={classId}", token);

    public Task<ApiResult<List<StudentGradeSummaryDto>>> GetMyAsync(string token)
        => GetAsync<List<StudentGradeSummaryDto>>("api/grades/my", token);

    public Task<ApiResult<bool>> BatchCreateAsync(string token, BatchGradeRequest request)
        => PostNoContentAsync("api/grades/batch", request, token);
}
