using Client.Services.Models;

namespace Client.Services;

public class ReportApiClient : BaseApiClient, IReportApiClient
{
    public ReportApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<DashboardDto>> GetDashboardAsync(string token)
        => GetAsync<DashboardDto>("api/reports/dashboard", token);
}
