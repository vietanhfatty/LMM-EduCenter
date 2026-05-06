using Client.Services.Models;

namespace Client.Services;

public interface IRoomApiClient
{
    Task<ApiResult<List<RoomDto>>> GetAllAsync(string token);
    Task<ApiResult<RoomDto>> GetByIdAsync(int id, string token);
    Task<ApiResult<RoomDto>> CreateAsync(string token, CreateRoomRequest request);
    Task<ApiResult<RoomDto>> UpdateAsync(int id, string token, UpdateRoomRequest request);
    Task<ApiResult<bool>> DeleteAsync(int id, string token);
}
