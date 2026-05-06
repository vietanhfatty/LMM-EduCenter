using Client.Services.Models;
using Microsoft.AspNetCore.Http;

namespace Client.Services;

public interface IAuthApiClient
{
    Task<ApiResult<AuthResponseDto>> LoginAsync(LoginApiRequest request);
    Task<ApiResult<UserProfileDto>> GetProfileAsync(string token);
    Task<ApiResult<UserProfileDto>> UpdateProfileAsync(string token, UpdateProfileApiRequest request);
    Task<ApiResult<bool>> ChangePasswordAsync(string token, ChangePasswordApiRequest request);
    Task<ApiResult<AvatarUploadResponseDto>> UploadAvatarAsync(string token, IFormFile file);
}
