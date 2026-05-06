using System.ComponentModel.DataAnnotations;

namespace Server.DTOs.Auth;

public class UpdateProfileRequest
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }
}
