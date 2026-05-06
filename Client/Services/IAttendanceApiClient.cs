using Client.Services.Models;

namespace Client.Services;

public interface IAttendanceApiClient
{
    Task<ApiResult<List<AttendanceDto>>> GetByClassAsync(int classId, string token, DateTime? date = null);
    Task<ApiResult<List<AttendanceDto>>> GetMyAsync(string token, int? classId = null);
    Task<ApiResult<bool>> BatchCreateAsync(string token, BatchAttendanceRequest request);
    Task<ApiResult<List<AttendanceReportDto>>> GetReportAsync(int classId, string token);
    Task<ApiResult<List<DateTime>>> GetDatesAsync(int classId, string token);
}
