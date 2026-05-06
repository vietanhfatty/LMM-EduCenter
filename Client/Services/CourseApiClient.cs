using Client.Services.Models;

namespace Client.Services;

public class CourseApiClient : BaseApiClient, ICourseApiClient
{
    public CourseApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<CourseDto>>> GetAllAsync(string token)
        => GetAsync<List<CourseDto>>("api/courses", token);

    public Task<ApiResult<CourseDto>> GetByIdAsync(int id, string token)
        => GetAsync<CourseDto>($"api/courses/{id}", token);

    public Task<ApiResult<CourseDto>> CreateAsync(string token, CreateCourseRequest request)
        => PostAsync<CourseDto>("api/courses", request, token);

    public Task<ApiResult<CourseDto>> UpdateAsync(int id, string token, UpdateCourseRequest request)
        => PutAsync<CourseDto>($"api/courses/{id}", request, token);

    public Task<ApiResult<bool>> DeleteAsync(int id, string token)
        => base.DeleteAsync($"api/courses/{id}", token);
}
