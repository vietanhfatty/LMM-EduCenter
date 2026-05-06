using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Client.Controllers;

[RequireRole("Teacher")]
public class TeacherMaterialController : BaseController
{
    private readonly ILearningMaterialApiClient _materialApi;
    private readonly IClassApiClient _classApi;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;

    public TeacherMaterialController(
        ILearningMaterialApiClient materialApi,
        IClassApiClient classApi,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _materialApi = materialApi;
        _classApi = classApi;
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    }

    public async Task<IActionResult> Index(int? classId = null)
    {
        var token = GetToken()!;
        var teacherId = GetUserId()!;

        var classesResult = await _classApi.GetAllAsync(token, teacherId);
        var classes = classesResult.Data?.Where(c => c.Status == 0 || c.Status == 1).ToList() ?? new List<ClassDto>();

        var materialResult = await _materialApi.GetTeacherAsync(token, classId);
        ViewBag.Classes = classes;
        ViewBag.SelectedClassId = classId;
        return View(materialResult.Data ?? new List<LearningMaterialDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(UploadLearningMaterialRequest model)
    {
        var token = GetToken()!;
        var result = await _materialApi.UploadAsync(token, model);

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Gửi tài liệu thất bại.";
            return RedirectToAction(nameof(Index), new { classId = model.ClassId });
        }

        TempData["Success"] = "Gửi tài liệu thành công.";
        return RedirectToAction(nameof(Index), new { classId = model.ClassId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int? classId = null)
    {
        var token = GetToken()!;
        var result = await _materialApi.DeleteAsync(id, token);

        if (!result.Success)
            TempData["Error"] = result.ErrorMessage ?? "Xóa tài liệu thất bại.";
        else
            TempData["Success"] = "Đã xóa tài liệu.";

        return RedirectToAction(nameof(Index), new { classId });
    }

    public async Task<IActionResult> Download(int id)
    {
        var token = GetToken()!;
        var materialResult = await _materialApi.GetTeacherAsync(token);
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
