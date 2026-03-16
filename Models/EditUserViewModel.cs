using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Administrator")]
    public bool IsAdministrator { get; set; }
}
