using Client.Services.Models;

namespace Client.Services;

public interface IGradeApiClient
{
    Task<ApiResult<List<GradeDto>>> GetByClassAsync(int classId, string token);
    Task<ApiResult<List<StudentGradeSummaryDto>>> GetMyAsync(string token);
    Task<ApiResult<bool>> BatchCreateAsync(string token, BatchGradeRequest request);
}
