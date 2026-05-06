using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Client.Controllers;

public abstract class BaseController : Controller
{
    protected const string TokenKey = "AuthToken";
    protected const string UserKey = "CurrentUser";

    protected string? GetToken() => HttpContext.Session.GetString(TokenKey);

    protected bool IsAuthenticated() => !string.IsNullOrWhiteSpace(GetToken());

    protected string? GetUserId()
    {
        var raw = HttpContext.Session.GetString(UserKey);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        using var doc = JsonDocument.Parse(raw);
        return doc.RootElement.GetProperty("UserId").GetString();
    }

    protected string? GetUserRole()
    {
        var raw = HttpContext.Session.GetString(UserKey);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        using var doc = JsonDocument.Parse(raw);
        var roles = doc.RootElement.GetProperty("Roles");
        return roles.GetArrayLength() > 0 ? roles[0].GetString() : null;
    }

    protected IActionResult RedirectToLogin()
    {
        TempData["Error"] = "Vui lòng đăng nhập.";
        return RedirectToAction("Login", "Auth");
    }

    protected IActionResult AccessDenied()
    {
        TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
        return RedirectToAction("Index", "Home");
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;
    public RequireRoleAttribute(string role) => _role = role;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        var token = session.GetString("AuthToken");
        if (string.IsNullOrWhiteSpace(token))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var raw = session.GetString("CurrentUser");
        if (string.IsNullOrWhiteSpace(raw))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        using var doc = JsonDocument.Parse(raw);
        var roles = doc.RootElement.GetProperty("Roles");
        bool hasRole = false;
        foreach (var r in roles.EnumerateArray())
        {
            if (r.GetString() == _role) { hasRole = true; break; }
        }

        if (!hasRole)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
