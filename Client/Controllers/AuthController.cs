using System.Text.Json;
using Client.Services;
using Client.Services.Models;
using Client.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

public class AuthController : Controller
{
    private const string TokenKey = "AuthToken";
    private const string UserKey = "CurrentUser";
    private readonly IAuthApiClient _authApiClient;

    public AuthController(IAuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (IsAuthenticated())
        {
            return RedirectToAction(nameof(Profile));
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authApiClient.LoginAsync(new LoginApiRequest
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đăng nhập thất bại.");
            return View(model);
        }

        SaveSession(result.Data);
        TempData["Success"] = "Đăng nhập thành công.";
        return RedirectByRole(result.Data.Roles);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString(TokenKey);
        if (string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _authApiClient.GetProfileAsync(token);
        if (!result.Success || result.Data is null)
        {
            ClearSession();
            TempData["Error"] = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
            return RedirectToAction(nameof(Login));
        }

        var viewModel = new ProfileViewModel
        {
            Id = result.Data.Id,
            Email = result.Data.Email,
            FullName = result.Data.FullName,
            Phone = result.Data.Phone,
            Avatar = result.Data.Avatar,
            DateOfBirth = result.Data.DateOfBirth,
            Address = result.Data.Address,
            IsActive = result.Data.IsActive,
            Roles = result.Data.Roles
        };

        ViewBag.ChangePasswordModel = new ChangePasswordViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var token = HttpContext.Session.GetString(TokenKey);
        if (string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ChangePasswordModel = new ChangePasswordViewModel();
            return View(model);
        }

        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            var uploadResult = await _authApiClient.UploadAvatarAsync(token, model.AvatarFile);
            if (!uploadResult.Success)
            {
                ModelState.AddModelError(string.Empty, uploadResult.ErrorMessage ?? "Tải ảnh đại diện thất bại.");
                ViewBag.ChangePasswordModel = new ChangePasswordViewModel();
                return View(model);
            }

            model.Avatar = uploadResult.Data?.Avatar;
        }

        var result = await _authApiClient.UpdateProfileAsync(token, new UpdateProfileApiRequest
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Avatar = model.Avatar,
            DateOfBirth = model.DateOfBirth,
            Address = model.Address
        });

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Cập nhật profile thất bại.");
            ViewBag.ChangePasswordModel = new ChangePasswordViewModel();
            return View(model);
        }

        UpdateSessionUser(result.Data.FullName, result.Data.Avatar);
        TempData["Success"] = "Cập nhật profile thành công.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var token = HttpContext.Session.GetString(TokenKey);
        if (string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            var profile = await BuildProfileForRetry(token);
            ViewBag.ChangePasswordModel = model;
            return View(nameof(Profile), profile);
        }

        var result = await _authApiClient.ChangePasswordAsync(token, new ChangePasswordApiRequest
        {
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword
        });

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Đổi mật khẩu thất bại.";
            return RedirectToAction(nameof(Profile));
        }

        TempData["Success"] = "Đổi mật khẩu thành công.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public IActionResult Logout()
    {
        ClearSession();
        TempData["Success"] = "Bạn đã đăng xuất.";
        return RedirectToAction(nameof(Login));
    }

    private bool IsAuthenticated() => !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(TokenKey));

    private void SaveSession(AuthResponseDto authResponse)
    {
        HttpContext.Session.SetString(TokenKey, authResponse.Token);
        var userJson = JsonSerializer.Serialize(new
        {
            authResponse.UserId,
            authResponse.Email,
            authResponse.FullName,
            authResponse.Avatar,
            authResponse.Roles
        });
        HttpContext.Session.SetString(UserKey, userJson);
    }

    private async Task<ProfileViewModel> BuildProfileForRetry(string token)
    {
        var result = await _authApiClient.GetProfileAsync(token);
        if (!result.Success || result.Data is null)
        {
            return new ProfileViewModel();
        }

        return new ProfileViewModel
        {
            Id = result.Data.Id,
            Email = result.Data.Email,
            FullName = result.Data.FullName,
            Phone = result.Data.Phone,
            Avatar = result.Data.Avatar,
            DateOfBirth = result.Data.DateOfBirth,
            Address = result.Data.Address,
            IsActive = result.Data.IsActive,
            Roles = result.Data.Roles
        };
    }

    private void UpdateSessionUser(string fullName, string? avatar)
    {
        var raw = HttpContext.Session.GetString(UserKey);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        using var document = JsonDocument.Parse(raw);
        var root = document.RootElement;
        var updated = new
        {
            UserId = root.GetProperty("UserId").GetString(),
            Email = root.GetProperty("Email").GetString(),
            FullName = fullName,
            Avatar = avatar,
            Roles = root.GetProperty("Roles").EnumerateArray().Select(x => x.GetString()).ToArray()
        };
        HttpContext.Session.SetString(UserKey, JsonSerializer.Serialize(updated));
    }

    private void ClearSession()
    {
        HttpContext.Session.Remove(TokenKey);
        HttpContext.Session.Remove(UserKey);
    }
    private IActionResult RedirectByRole(IReadOnlyList<string> roles)
    {
        if (roles.Any(r => r == "Staff"))
            return RedirectToAction("Index", "StaffDashboard");

        if (roles.Any(r => r == "Teacher"))
            return RedirectToAction("Index", "TeacherDashboard");

        if (roles.Any(r => r == "Student"))
            return RedirectToAction("Index", "StudentDashboard");

        return RedirectToAction(nameof(Profile));
    }
}