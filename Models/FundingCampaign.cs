using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Fund_Project.Models;

public class FundingCampaign
{
    public int Id { get; set; }

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
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Goal amount")]
    public decimal GoalAmount { get; set; }

    [Range(typeof(decimal), "0", "500000", ErrorMessage = "Current amount must be between 0 and 500000.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Current amount")]
    public decimal CurrentAmount { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End date")]
    public DateTime EndDate { get; set; }

    [Display(Name = "Featured campaign")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Approved by admin")]
    public bool IsApproved { get; set; }

    [Display(Name = "Hidden from public view")]
    public bool IsHidden { get; set; }

    [Display(Name = "Deletion requested")]
    public bool IsDeletionRequested { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Submitted on")]
    public DateTime CreatedOn { get; set; }

    [Display(Name = "Beneficiary")]
    public int BeneficiaryId { get; set; }

    public Beneficiary? Beneficiary { get; set; }

    public string OwnerId { get; set; } = string.Empty;

    public ApplicationUser? Owner { get; set; }

    public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
}
