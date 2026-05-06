using Client.Services.Models;

namespace Client.Services;

public interface INotificationApiClient
{
    Task<ApiResult<List<NotificationDto>>> GetAllAsync(string token);
    Task<ApiResult<NotificationDto>> GetByIdAsync(int id, string token);
    Task<ApiResult<NotificationDto>> CreateAsync(string token, CreateNotificationRequest request);
    Task<ApiResult<bool>> MarkReadAsync(int id, string token);
}
