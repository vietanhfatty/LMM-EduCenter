using Client.Services.Models;

namespace Client.Services;

public class NotificationApiClient : BaseApiClient, INotificationApiClient
{
    public NotificationApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<NotificationDto>>> GetAllAsync(string token)
        => GetAsync<List<NotificationDto>>("api/notifications", token);

    public Task<ApiResult<NotificationDto>> GetByIdAsync(int id, string token)
        => GetAsync<NotificationDto>($"api/notifications/{id}", token);

    public Task<ApiResult<NotificationDto>> CreateAsync(string token, CreateNotificationRequest request)
        => PostAsync<NotificationDto>("api/notifications", request, token);

    public Task<ApiResult<bool>> MarkReadAsync(int id, string token)
        => PostNoContentAsync($"api/notifications/{id}/read", new { }, token);
}
