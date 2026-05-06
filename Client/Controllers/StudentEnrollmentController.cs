using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentEnrollmentController : BaseController
{
    private readonly IEnrollmentApiClient _enrollmentApi;
    private const int DefaultPageSize = 10;

    public StudentEnrollmentController(IEnrollmentApiClient enrollmentApi) => _enrollmentApi = enrollmentApi;

    public async Task<IActionResult> Index(int? status, string? keyword, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.GetMyAsync(token);
        var enrollments = result.Data ?? new List<EnrollmentDto>();

        if (status.HasValue)
            enrollments = enrollments.Where(e => e.Status == status.Value).ToList();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            enrollments = enrollments.Where(e =>
                e.ClassName.ToLower().Contains(normalized) ||
                e.CourseName.ToLower().Contains(normalized) ||
                e.TeacherName.ToLower().Contains(normalized)).ToList();
        }

        var totalItems = enrollments.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = enrollments
            .OrderByDescending(e => e.EnrollDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Status = status;
        ViewBag.Keyword = keyword;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int classId)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.CreateAsync(token, new CreateEnrollmentRequest { ClassId = classId });

        if (!result.Success)
        {
            var message = result.ErrorMessage ?? "Đăng ký thất bại.";
            if (message.Contains("Mỗi khóa học chỉ được", StringComparison.OrdinalIgnoreCase))
            {
                TempData["PopupError"] = "Bạn đã đăng ký 1 khóa học này!";
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectBackToCoursePage();
        }

        TempData["Success"] = "Đăng ký thành công! Vui lòng chờ Staff duyệt.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var token = GetToken()!;
        var result = await _enrollmentApi.CancelAsync(id, token);

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Hủy đăng ký thất bại.";
        }
        else
        {
            TempData["Success"] = "Đã hủy đăng ký.";
        }

        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectBackToCoursePage()
    {
        var referer = Request.Headers.Referer.ToString();
        if (Uri.TryCreate(referer, UriKind.Absolute, out var uri))
        {
            var localPath = uri.PathAndQuery + uri.Fragment;
            if (Url.IsLocalUrl(localPath))
                return LocalRedirect(localPath);
        }

        return RedirectToAction("Index", "StudentCourse");
    }
}
