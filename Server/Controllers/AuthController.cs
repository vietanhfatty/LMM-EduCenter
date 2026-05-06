using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.DTOs.Auth;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public AuthController(
        UserManager<AppUser> userManager,
        IJwtService jwt,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        _userManager = userManager;
        _jwt = jwt;
        _configuration = configuration;
        _env = env;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

        var ok = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(BuildAuthResponse(user, roles));
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(MapProfile(user, roles));
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.Avatar = request.Avatar;
        user.DateOfBirth = request.DateOfBirth;
        user.Address = request.Address;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(MapProfile(user, roles));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        return NoContent();
    }

    [HttpPost("avatar")]
    [Authorize]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<AvatarUploadResponse>> UploadAvatar([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Vui lòng chọn file ảnh." });

        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Chỉ chấp nhận file ảnh." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var root = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(root))
            root = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        var folder = Path.Combine(root, "avatars", "users");
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext))
            ext = ".png";
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.Avatar = $"{Request.Scheme}://{Request.Host}/avatars/users/{fileName}";
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        return Ok(new AvatarUploadResponse { Avatar = user.Avatar });
    }

    private AuthResponse BuildAuthResponse(AppUser user, IList<string> roles)
    {
        var token = _jwt.CreateToken(user, roles);
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;
        return new AuthResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes),
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Avatar = user.Avatar,
            Roles = roles.ToList()
        };
    }

    private static UserProfileDto MapProfile(AppUser user, IList<string> roles) =>
        new()
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Phone = user.Phone,
            Avatar = user.Avatar,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            IsActive = user.IsActive,
            Roles = roles.ToList()
        };
}

public class AvatarUploadResponse
{
    public string? Avatar { get; set; }
}
