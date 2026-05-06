using Client.Services.Models;

namespace Client.Services;

public class RoomApiClient : BaseApiClient, IRoomApiClient
{
    public RoomApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<List<RoomDto>>> GetAllAsync(string token)
        => GetAsync<List<RoomDto>>("api/rooms", token);

    public Task<ApiResult<RoomDto>> GetByIdAsync(int id, string token)
        => GetAsync<RoomDto>($"api/rooms/{id}", token);

    public Task<ApiResult<RoomDto>> CreateAsync(string token, CreateRoomRequest request)
        => PostAsync<RoomDto>("api/rooms", request, token);

    public Task<ApiResult<RoomDto>> UpdateAsync(int id, string token, UpdateRoomRequest request)
        => PutAsync<RoomDto>($"api/rooms/{id}", request, token);

    public Task<ApiResult<bool>> DeleteAsync(int id, string token)
        => base.DeleteAsync($"api/rooms/{id}", token);
}
