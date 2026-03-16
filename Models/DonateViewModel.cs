using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public class DonateViewModel
{
    public int FundingCampaignId { get; set; }

    public string CampaignTitle { get; set; } = string.Empty;

    public string BeneficiaryName { get; set; } = string.Empty;

    public string BankAccountName { get; set; } = string.Empty;

    public string BankName { get; set; } = string.Empty;

    public string Iban { get; set; } = string.Empty;

    public string Bic { get; set; } = string.Empty;

    [Display(Name = "Donor name")]
    [StringLength(80, MinimumLength = 2)]
    public string? DonorName { get; set; }

    [Display(Name = "Make this donation anonymous")]
    public bool IsAnonymous { get; set; }

    [Display(Name = "Hide this donation from other visitors")]
    public bool IsHiddenDonation { get; set; }

    [Required]
    [Range(typeof(decimal), "5", "100000", ErrorMessage = "Contribution amount must be between 5 and 100000.")]
    public decimal Amount { get; set; }

    [StringLength(240)]
    public string? Note { get; set; }

    [Display(Name = "Payment method")]
    public ContributionPaymentMethod PaymentMethod { get; set; } = ContributionPaymentMethod.Card;

    [Display(Name = "Donation frequency")]
    public ContributionFrequency Frequency { get; set; } = ContributionFrequency.OneTime;

    [Display(Name = "Card test result")]
    public string CardTestOutcome { get; set; } = "success";
}
