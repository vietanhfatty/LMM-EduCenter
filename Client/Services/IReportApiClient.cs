using Client.Services.Models;

namespace Client.Services;

public interface IReportApiClient
{
    Task<ApiResult<DashboardDto>> GetDashboardAsync(string token);
}
