using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASP_Fund_Project.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    public ICollection<FundingCampaign> CreatedCampaigns { get; set; } = new List<FundingCampaign>();

    public ICollection<Beneficiary> ManagedBeneficiaries { get; set; } = new List<Beneficiary>();

    public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
}
