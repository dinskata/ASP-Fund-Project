using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public class OrganizationRegistrationViewModel
{
    [Required]
    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Organization name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(60, MinimumLength = 3)]
    [Display(Name = "What it does")]
    public string FocusArea { get; set; } = string.Empty;

    [Required]
    [StringLength(60, MinimumLength = 2)]
    [Display(Name = "Town/City")]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(600, MinimumLength = 30)]
    [Display(Name = "Organization information")]
    public string Story { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Account holder")]
    public string BankAccountName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Bank name")]
    public string BankName { get; set; } = string.Empty;

    [Required]
    [StringLength(34, MinimumLength = 8)]
    [Display(Name = "IBAN")]
    public string Iban { get; set; } = string.Empty;

    [Required]
    [StringLength(11, MinimumLength = 8)]
    [Display(Name = "BIC")]
    public string Bic { get; set; } = string.Empty;
}
