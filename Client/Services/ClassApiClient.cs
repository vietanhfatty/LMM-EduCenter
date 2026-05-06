using Client.Services.Models;

namespace Client.Services;

public class ClassApiClient : BaseApiClient, IClassApiClient
{
    public ClassApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<ClassDto>>> GetAllAsync(string token, string? teacherId = null)
    {
        var path = "api/classes";
        if (!string.IsNullOrEmpty(teacherId))
            path += $"?teacherId={teacherId}";
        return GetAsync<List<ClassDto>>(path, token);
    }

    public Task<ApiResult<ClassDto>> GetByIdAsync(int id, string token)
        => GetAsync<ClassDto>($"api/classes/{id}", token);

    public Task<ApiResult<ClassDto>> CreateAsync(string token, CreateClassRequest request)
        => PostAsync<ClassDto>("api/classes", request, token);

    public Task<ApiResult<ClassDto>> UpdateAsync(int id, string token, UpdateClassRequest request)
        => PutAsync<ClassDto>($"api/classes/{id}", request, token);

    public Task<ApiResult<bool>> DeleteAsync(int id, string token)
        => base.DeleteAsync($"api/classes/{id}", token);
}
