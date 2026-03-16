using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Fund_Project.Models;

public class Contribution
{
    public int Id { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Donor name")]
    public string DonorName { get; set; } = string.Empty;

    [Range(typeof(decimal), "5", "100000", ErrorMessage = "Contribution amount must be between 5 and 100000.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Contribution date")]
    public DateTime DonatedOn { get; set; }

    [StringLength(240)]
    public string? Note { get; set; }

    public bool IsCommentHidden { get; set; }

    public bool IsDonationHidden { get; set; }

    [Display(Name = "Payment method")]
    public ContributionPaymentMethod PaymentMethod { get; set; }

    [Display(Name = "Donation frequency")]
    public ContributionFrequency Frequency { get; set; }

    [Display(Name = "Payment status")]
    public ContributionStatus Status { get; set; }

    public int FundingCampaignId { get; set; }

    public FundingCampaign? FundingCampaign { get; set; }

    public string? ContributorUserId { get; set; }

    public ApplicationUser? ContributorUser { get; set; }
}
