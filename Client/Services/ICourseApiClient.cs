using Client.Services.Models;

namespace Client.Services;

public interface ICourseApiClient
{
    Task<ApiResult<List<CourseDto>>> GetAllAsync(string token);
    Task<ApiResult<CourseDto>> GetByIdAsync(int id, string token);
    Task<ApiResult<CourseDto>> CreateAsync(string token, CreateCourseRequest request);
    Task<ApiResult<CourseDto>> UpdateAsync(int id, string token, UpdateCourseRequest request);
    Task<ApiResult<bool>> DeleteAsync(int id, string token);
}
