using Client.Services.Models;

namespace Client.Services;

public interface IClassApiClient
{
    Task<ApiResult<List<ClassDto>>> GetAllAsync(string token, string? teacherId = null);
    Task<ApiResult<ClassDto>> GetByIdAsync(int id, string token);
    Task<ApiResult<ClassDto>> CreateAsync(string token, CreateClassRequest request);
    Task<ApiResult<ClassDto>> UpdateAsync(int id, string token, UpdateClassRequest request);
    Task<ApiResult<bool>> DeleteAsync(int id, string token);
}
