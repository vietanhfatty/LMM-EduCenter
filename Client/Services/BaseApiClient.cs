using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Services.Models;

namespace Client.Services;

public abstract class BaseApiClient
{
    protected readonly HttpClient Http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected BaseApiClient(HttpClient httpClient)
    {
        Http = httpClient;
    }

    protected async Task<ApiResult<T>> GetAsync<T>(string path, string? token = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            AttachBearer(request, token);
            var response = await Http.SendAsync(request);
            return await HandleAsync<T>(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return CreateConnectionError<T>(ex);
        }
    }

    protected async Task<ApiResult<T>> PostAsync<T>(string path, object body, string? token = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(body)
            };
            AttachBearer(request, token);
            var response = await Http.SendAsync(request);
            return await HandleAsync<T>(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return CreateConnectionError<T>(ex);
        }
    }

    protected async Task<ApiResult<T>> PutAsync<T>(string path, object body, string? token = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, path)
            {
                Content = JsonContent.Create(body)
            };
            AttachBearer(request, token);
            var response = await Http.SendAsync(request);
            return await HandleAsync<T>(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return CreateConnectionError<T>(ex);
        }
    }

    protected async Task<ApiResult<bool>> DeleteAsync(string path, string? token = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, path);
            AttachBearer(request, token);
            var response = await Http.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return new ApiResult<bool> { Success = true, Data = true };

            return new ApiResult<bool>
            {
                Success = false,
                ErrorMessage = await ReadErrorAsync(response),
                Data = false
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ApiResult<bool>
            {
                Success = false,
                Data = false,
                ErrorMessage = CreateConnectionErrorMessage(ex)
            };
        }
    }

    protected async Task<ApiResult<bool>> PostNoContentAsync(string path, object body, string? token = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(body)
            };
            AttachBearer(request, token);
            var response = await Http.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return new ApiResult<bool> { Success = true, Data = true };

            return new ApiResult<bool>
            {
                Success = false,
                ErrorMessage = await ReadErrorAsync(response),
                Data = false
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new ApiResult<bool>
            {
                Success = false,
                Data = false,
                ErrorMessage = CreateConnectionErrorMessage(ex)
            };
        }
    }

    private static void AttachBearer(HttpRequestMessage request, string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<ApiResult<T>> HandleAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            return new ApiResult<T> { Success = true, Data = data };
        }

        return new ApiResult<T>
        {
            Success = false,
            ErrorMessage = await ReadErrorAsync(response),
            Data = default
        };
    }

    protected static async Task<string> ReadErrorAsync(HttpResponseMessage response)
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

    private static ApiResult<T> CreateConnectionError<T>(Exception ex)
    {
        return new ApiResult<T>
        {
            Success = false,
            Data = default,
            ErrorMessage = CreateConnectionErrorMessage(ex)
        };
    }

    private static string CreateConnectionErrorMessage(Exception ex) =>
        $"Khong the ket noi toi API ({ex.Message}). Hay khoi dong Server va thu lai.";
}
