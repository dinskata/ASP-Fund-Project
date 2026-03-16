using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public class CreateCampaignViewModel
{
    [Required]
    [StringLength(90, MinimumLength = 4)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public CampaignCategory Category { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Organizer")]
    public string OrganizerName { get; set; } = string.Empty;

    [Required]
    [StringLength(700, MinimumLength = 40)]
    public string Summary { get; set; } = string.Empty;

    [Range(typeof(decimal), "50", "500000", ErrorMessage = "Goal amount must be between 50 and 500000.")]
    [Display(Name = "Goal amount")]
    public decimal GoalAmount { get; set; }

    [Range(typeof(decimal), "0", "500000", ErrorMessage = "Current amount must be between 0 and 500000.")]
    [Display(Name = "Current amount")]
    public decimal CurrentAmount { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End date")]
    public DateTime EndDate { get; set; }

    [Display(Name = "Select beneficiary")]
    public string CreationMode { get; set; } = "self";

    [Display(Name = "Approved organization")]
    public int? BeneficiaryId { get; set; }

    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Your name")]
    public string? SelfBeneficiaryName { get; set; }

    [StringLength(60, MinimumLength = 3)]
    [Display(Name = "What it's for")]
    public string? SelfFocusArea { get; set; }

    [StringLength(60, MinimumLength = 2)]
    [Display(Name = "Town/City")]
    public string? SelfCity { get; set; }

    [StringLength(600, MinimumLength = 30)]
    [Display(Name = "More information")]
    public string? SelfStory { get; set; }

    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Account holder")]
    public string? SelfBankAccountName { get; set; }

    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Bank name")]
    public string? SelfBankName { get; set; }

    [StringLength(34, MinimumLength = 8)]
    [Display(Name = "IBAN")]
    public string? SelfIban { get; set; }

    [StringLength(11, MinimumLength = 8)]
    [Display(Name = "BIC")]
    public string? SelfBic { get; set; }

    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Beneficiary name")]
    public string? AnotherBeneficiaryName { get; set; }

    [Display(Name = "Beneficiary type")]
    public BeneficiaryKind AnotherBeneficiaryKind { get; set; } = BeneficiaryKind.Person;

    [StringLength(60, MinimumLength = 3)]
    [Display(Name = "What it's for")]
    public string? AnotherFocusArea { get; set; }

    [StringLength(60, MinimumLength = 2)]
    [Display(Name = "Town/City")]
    public string? AnotherCity { get; set; }

    [StringLength(600, MinimumLength = 30)]
    [Display(Name = "More information")]
    public string? AnotherStory { get; set; }

    [StringLength(80, MinimumLength = 3)]
    [Display(Name = "Account holder")]
    public string? AnotherBankAccountName { get; set; }

    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Bank name")]
    public string? AnotherBankName { get; set; }

    [StringLength(34, MinimumLength = 8)]
    [Display(Name = "IBAN")]
    public string? AnotherIban { get; set; }

    [StringLength(11, MinimumLength = 8)]
    [Display(Name = "BIC")]
    public string? AnotherBic { get; set; }
}
