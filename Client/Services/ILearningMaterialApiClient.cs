using Client.Services.Models;

namespace Client.Services;

public interface ILearningMaterialApiClient
{
    Task<ApiResult<List<LearningMaterialDto>>> GetTeacherAsync(string token, int? classId = null);
    Task<ApiResult<List<LearningMaterialDto>>> GetStudentAsync(string token, int? classId = null);
    Task<ApiResult<LearningMaterialDto>> UploadAsync(string token, UploadLearningMaterialRequest request);
    Task<ApiResult<bool>> DeleteAsync(int id, string token);
}
