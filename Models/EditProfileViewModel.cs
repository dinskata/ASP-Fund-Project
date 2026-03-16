using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public class EditProfileViewModel
{
    [Required]
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

    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "New password")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmNewPassword { get; set; }

    public IReadOnlyCollection<Beneficiary> RegisteredOrganizations { get; set; } = [];

    public IReadOnlyCollection<FundingCampaign> MyCauses { get; set; } = [];

    public IReadOnlyCollection<Contribution> MyDonations { get; set; } = [];
}
