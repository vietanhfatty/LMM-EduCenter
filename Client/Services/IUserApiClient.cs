using Client.Services.Models;

namespace Client.Services;

public interface IUserApiClient
{
    Task<ApiResult<List<UserDto>>> GetTeachersAsync(string token);
    Task<ApiResult<UserDto>> GetTeacherByIdAsync(string id, string token);
    Task<ApiResult<UserDto>> CreateTeacherAsync(string token, CreateUserRequest request);
    Task<ApiResult<UserDto>> UpdateTeacherAsync(string id, string token, UpdateUserRequest request);
    Task<ApiResult<bool>> ToggleTeacherAsync(string id, string token);

    Task<ApiResult<List<UserDto>>> GetStudentsAsync(string token);
    Task<ApiResult<UserDto>> GetStudentByIdAsync(string id, string token);
    Task<ApiResult<UserDto>> CreateStudentAsync(string token, CreateUserRequest request);
    Task<ApiResult<UserDto>> UpdateStudentAsync(string id, string token, UpdateUserRequest request);
    Task<ApiResult<bool>> ToggleStudentAsync(string id, string token);
}
