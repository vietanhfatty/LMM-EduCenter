using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffPaymentController : BaseController
{
    private readonly IPaymentApiClient _paymentApi;
    private readonly IUserApiClient _userApi;
    private readonly IEnrollmentApiClient _enrollmentApi;

    public StaffPaymentController(
        IPaymentApiClient paymentApi,
        IUserApiClient userApi,
        IEnrollmentApiClient enrollmentApi)
    {
        _paymentApi = paymentApi;
        _userApi = userApi;
        _enrollmentApi = enrollmentApi;
    }

    public async Task<IActionResult> Index(string? keyword, int? method, int? status, int page = 1, int pageSize = 10)
    {
        var token = GetToken()!;
        var result = await _paymentApi.GetAllAsync(token);
        var payments = result.Data ?? new List<PaymentDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            payments = payments.Where(p =>
                p.StudentName.ToLower().Contains(normalized) ||
                p.ClassName.ToLower().Contains(normalized) ||
                p.CourseName.ToLower().Contains(normalized)).ToList();
        }

        if (method.HasValue)
            payments = payments.Where(p => p.Method == method.Value).ToList();
        if (status.HasValue)
            payments = payments.Where(p => p.Status == status.Value).ToList();

        var totalItems = payments.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = payments
            .OrderByDescending(p => p.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Keyword = keyword;
        ViewBag.Method = method;
        ViewBag.Status = status;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    public async Task<IActionResult> Create()
    {
        var token = GetToken()!;
        var studentResult = await _userApi.GetStudentsAsync(token);
        var enrollmentResult = await _enrollmentApi.GetAllAsync(token, status: 1);
        var debtResult = await _paymentApi.GetDebtsAsync(token);

        ViewBag.Students = studentResult.Data ?? new List<UserDto>();
        ViewBag.Enrollments = enrollmentResult.Data ?? new List<EnrollmentDto>();
        ViewBag.Debts = debtResult.Data ?? new List<DebtDto>();
        return View("CreateFixed", new CreatePaymentRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePaymentRequest model)
    {
        var token = GetToken()!;
        var result = await _paymentApi.CreateAsync(token, model);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Ghi nhận thanh toán thất bại.";
            return RedirectToAction(nameof(Create));
        }

        TempData["Success"] = "Ghi nhận thanh toán tiền mặt thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmTransfer(int id)
    {
        var token = GetToken()!;
        var result = await _paymentApi.ConfirmTransferAsync(token, id);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Xác nhận chuyển khoản thất bại.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Đã xác nhận chuyển khoản thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Debts()
    {
        var token = GetToken()!;
        var result = await _paymentApi.GetDebtsAsync(token);
        return View(result.Data ?? new List<DebtDto>());
    }

    public async Task<IActionResult> Revenue(int? year = null)
    {
        var token = GetToken()!;
        var result = await _paymentApi.GetRevenueAsync(token, year);
        return View(result.Data);
    }
}
