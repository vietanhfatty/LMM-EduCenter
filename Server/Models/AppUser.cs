using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class AppUser : IdentityUser
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
