using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Student")]
public class StudentPaymentController : BaseController
{
    private readonly IPaymentApiClient _paymentApi;
    private const int DefaultPageSize = 10;

    public StudentPaymentController(IPaymentApiClient paymentApi) => _paymentApi = paymentApi;

    public async Task<IActionResult> Index(string? keyword, int? method, int? status, int page = 1, int pageSize = DefaultPageSize)
    {
        var token = GetToken()!;
        var paymentResult = await _paymentApi.GetMyAsync(token);
        var debtResult = await _paymentApi.GetMyDebtsAsync(token);
        var payments = paymentResult.Data ?? new List<PaymentDto>();
        var debts = debtResult.Data ?? new List<DebtDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            payments = payments.Where(p =>
                p.ClassName.ToLower().Contains(normalized) ||
                p.CourseName.ToLower().Contains(normalized) ||
                (p.Note != null && p.Note.ToLower().Contains(normalized))).ToList();
            debts = debts.Where(d =>
                d.ClassName.ToLower().Contains(normalized)).ToList();
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

        ViewBag.Debts = debts;
        ViewBag.Keyword = keyword;
        ViewBag.Method = method;
        ViewBag.Status = status;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTransfer(CreateBankTransferRequest model)
    {
        var token = GetToken()!;
        var result = await _paymentApi.CreateMyBankTransferAsync(token, model);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Không thể tạo yêu cầu chuyển khoản.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Đã tạo yêu cầu chuyển khoản. Vui lòng chờ staff xác nhận.";
        return RedirectToAction(nameof(Index));
    }
}
