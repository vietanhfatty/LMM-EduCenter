using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Services.Models;

namespace Client.Services;

public class LearningMaterialApiClient : ILearningMaterialApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LearningMaterialApiClient(HttpClient http) => _http = http;

    public Task<ApiResult<List<LearningMaterialDto>>> GetTeacherAsync(string token, int? classId = null)
    {
        var path = "api/learning-materials/teacher";
        if (classId.HasValue) path += $"?classId={classId.Value}";
        return GetAsync<List<LearningMaterialDto>>(path, token);
    }

    public Task<ApiResult<List<LearningMaterialDto>>> GetStudentAsync(string token, int? classId = null)
    {
        var path = "api/learning-materials/student";
        if (classId.HasValue) path += $"?classId={classId.Value}";
        return GetAsync<List<LearningMaterialDto>>(path, token);
    }

    public async Task<ApiResult<LearningMaterialDto>> UploadAsync(string token, UploadLearningMaterialRequest request)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.ClassId.ToString()), "ClassId");
            content.Add(new StringContent(request.Title ?? string.Empty), "Title");
            content.Add(new StringContent(request.Description ?? string.Empty), "Description");

            if (request.File != null && request.File.Length > 0)
            {
                var streamContent = new StreamContent(request.File.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.File.ContentType ?? "application/octet-stream");
                content.Add(streamContent, "File", request.File.FileName);
            }

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/learning-materials")
            {
                Content = content
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<LearningMaterialDto>
                {
                    Success = false,
                    ErrorMessage = await ReadErrorAsync(response)
                };
            }

            var data = await response.Content.ReadFromJsonAsync<LearningMaterialDto>(JsonOptions);
            return new ApiResult<LearningMaterialDto> { Success = true, Data = data };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ApiResult<LearningMaterialDto>
            {
                Success = false,
                ErrorMessage = $"Khong the ket noi toi API ({ex.Message}). Hay khoi dong Server va thu lai."
            };
        }
    }

    public async Task<ApiResult<bool>> DeleteAsync(int id, string token)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/learning-materials/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return new ApiResult<bool> { Success = true, Data = true };

            return new ApiResult<bool>
            {
                Success = false,
                Data = false,
                ErrorMessage = await ReadErrorAsync(response)
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ApiResult<bool>
            {
                Success = false,
                Data = false,
                ErrorMessage = $"Khong the ket noi toi API ({ex.Message}). Hay khoi dong Server va thu lai."
            };
        }
    }

    private async Task<ApiResult<T>> GetAsync<T>(string path, string token)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<T>
                {
                    Success = false,
                    ErrorMessage = await ReadErrorAsync(response)
                };
            }

            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            return new ApiResult<T> { Success = true, Data = data };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ApiResult<T>
            {
                Success = false,
                ErrorMessage = $"Khong the ket noi toi API ({ex.Message}). Hay khoi dong Server va thu lai."
            };
        }
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var raw = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(raw))
            return $"API call failed: {(int)response.StatusCode} {response.ReasonPhrase}";

        try
        {
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.TryGetProperty("message", out var message))
                return message.GetString() ?? raw;
        }
        catch { }

        return raw;
    }
}
