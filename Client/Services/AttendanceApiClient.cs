using Client.Services.Models;

namespace Client.Services;

public class AttendanceApiClient : BaseApiClient, IAttendanceApiClient
{
    public AttendanceApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<AttendanceDto>>> GetByClassAsync(int classId, string token, DateTime? date = null)
    {
        var path = $"api/attendances?classId={classId}";
        if (date.HasValue) path += $"&date={date.Value:yyyy-MM-dd}";
        return GetAsync<List<AttendanceDto>>(path, token);
    }

    public Task<ApiResult<List<AttendanceDto>>> GetMyAsync(string token, int? classId = null)
    {
        var path = "api/attendances/my";
        if (classId.HasValue) path += $"?classId={classId}";
        return GetAsync<List<AttendanceDto>>(path, token);
    }

    public Task<ApiResult<bool>> BatchCreateAsync(string token, BatchAttendanceRequest request)
        => PostNoContentAsync("api/attendances/batch", request, token);

    public Task<ApiResult<List<AttendanceReportDto>>> GetReportAsync(int classId, string token)
        => GetAsync<List<AttendanceReportDto>>($"api/attendances/report/{classId}", token);

    public Task<ApiResult<List<DateTime>>> GetDatesAsync(int classId, string token)
        => GetAsync<List<DateTime>>($"api/attendances/dates/{classId}", token);
}
