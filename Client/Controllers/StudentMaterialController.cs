using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentMaterialController : BaseController
{
    private readonly ILearningMaterialApiClient _materialApi;
    private readonly IEnrollmentApiClient _enrollmentApi;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;

    public StudentMaterialController(
        ILearningMaterialApiClient materialApi,
        IEnrollmentApiClient enrollmentApi,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _materialApi = materialApi;
        _enrollmentApi = enrollmentApi;
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    }

    public async Task<IActionResult> Index(int? classId = null)
    {
        var token = GetToken()!;
        var enrollmentResult = await _enrollmentApi.GetMyAsync(token);
        var classes = enrollmentResult.Data?
            .Where(e => e.Status == 1)
            .GroupBy(e => e.ClassId)
            .Select(g => g.First())
            .OrderBy(e => e.ClassName)
            .ToList() ?? new List<EnrollmentDto>();

        var materialResult = await _materialApi.GetStudentAsync(token, classId);
        ViewBag.Classes = classes;
        ViewBag.SelectedClassId = classId;
        return View(materialResult.Data ?? new List<LearningMaterialDto>());
    }

    public async Task<IActionResult> Download(int id)
    {
        var token = GetToken()!;
        var materialResult = await _materialApi.GetStudentAsync(token);
        var material = materialResult.Data?.FirstOrDefault(x => x.Id == id);
        if (material == null)
        {
            TempData["Error"] = "Không tìm thấy tài liệu.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl.TrimEnd('/')}/api/learning-materials/{id}/download");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không tải được file tài liệu.";
                return RedirectToAction(nameof(Index));
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return File(bytes, "application/octet-stream", material.OriginalFileName);
        }
        catch
        {
            TempData["Error"] = "Không tải được file tài liệu.";
            return RedirectToAction(nameof(Index));
        }
    }
}
