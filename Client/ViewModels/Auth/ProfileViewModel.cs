using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Client.ViewModels.Auth;

public class ProfileViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự.")]
    public string? Phone { get; set; }

    public string? Avatar { get; set; }
    public IFormFile? AvatarFile { get; set; }
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự.")]
    public string? Address { get; set; }

    public bool IsActive { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
