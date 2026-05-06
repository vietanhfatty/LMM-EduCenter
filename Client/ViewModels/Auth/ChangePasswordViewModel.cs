using System.ComponentModel.DataAnnotations;

namespace Client.ViewModels.Auth;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu mới là bắt buộc.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
