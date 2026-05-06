using Client.Services.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Services;

public class AuthApiClient : BaseApiClient, IAuthApiClient
{
    public AuthApiClient(HttpClient httpClient) : base(httpClient) { }

    public Task<ApiResult<AuthResponseDto>> LoginAsync(LoginApiRequest request)
        => PostAsync<AuthResponseDto>("api/auth/login", request);

    public Task<ApiResult<UserProfileDto>> GetProfileAsync(string token)
        => GetAsync<UserProfileDto>("api/auth/profile", token);

    public Task<ApiResult<UserProfileDto>> UpdateProfileAsync(string token, UpdateProfileApiRequest request)
        => PutAsync<UserProfileDto>("api/auth/profile", request, token);

    public Task<ApiResult<bool>> ChangePasswordAsync(string token, ChangePasswordApiRequest request)
        => PostNoContentAsync("api/auth/change-password", request, token);

    public async Task<ApiResult<AvatarUploadResponseDto>> UploadAvatarAsync(string token, IFormFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "file", file.FileName);

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/avatar")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<AvatarUploadResponseDto>
                {
                    Success = false,
                    ErrorMessage = await ReadErrorAsync(response)
                };
            }

            var data = await response.Content.ReadFromJsonAsync<AvatarUploadResponseDto>();
            return new ApiResult<AvatarUploadResponseDto> { Success = true, Data = data };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ApiResult<AvatarUploadResponseDto>
            {
                Success = false,
                ErrorMessage = $"Khong the ket noi toi API ({ex.Message}). Hay khoi dong Server va thu lai."
            };
        }
    }
}
