using Client.Services.Models;

namespace Client.Services;

public class UserApiClient : BaseApiClient, IUserApiClient
{
    public UserApiClient(HttpClient httpClient) : base(httpClient) { }

    // Teachers
    public Task<ApiResult<List<UserDto>>> GetTeachersAsync(string token)
        => GetAsync<List<UserDto>>("api/teachers", token);

    public Task<ApiResult<UserDto>> GetTeacherByIdAsync(string id, string token)
        => GetAsync<UserDto>($"api/teachers/{id}", token);

    public Task<ApiResult<UserDto>> CreateTeacherAsync(string token, CreateUserRequest request)
        => PostAsync<UserDto>("api/teachers", request, token);

    public Task<ApiResult<UserDto>> UpdateTeacherAsync(string id, string token, UpdateUserRequest request)
        => PutAsync<UserDto>($"api/teachers/{id}", request, token);

    public Task<ApiResult<bool>> ToggleTeacherAsync(string id, string token)
        => PostNoContentAsync($"api/teachers/{id}/toggle", new { }, token);

    // Students
    public Task<ApiResult<List<UserDto>>> GetStudentsAsync(string token)
        => GetAsync<List<UserDto>>("api/students", token);

    public Task<ApiResult<UserDto>> GetStudentByIdAsync(string id, string token)
        => GetAsync<UserDto>($"api/students/{id}", token);

    public Task<ApiResult<UserDto>> CreateStudentAsync(string token, CreateUserRequest request)
        => PostAsync<UserDto>("api/students", request, token);

    public Task<ApiResult<UserDto>> UpdateStudentAsync(string id, string token, UpdateUserRequest request)
        => PutAsync<UserDto>($"api/students/{id}", request, token);

    public Task<ApiResult<bool>> ToggleStudentAsync(string id, string token)
        => PostNoContentAsync($"api/students/{id}/toggle", new { }, token);
}
