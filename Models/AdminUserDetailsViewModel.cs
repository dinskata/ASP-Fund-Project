namespace ASP_Fund_Project.Models;

public class AdminUserDetailsViewModel
{
    public ApplicationUser User { get; set; } = null!;

    public bool IsAdministrator { get; set; }

    public IReadOnlyCollection<FundingCampaign> Causes { get; set; } = [];

    public IReadOnlyCollection<Beneficiary> Organizations { get; set; } = [];

    public IReadOnlyCollection<Contribution> Donations { get; set; } = [];

    public IReadOnlyCollection<Contribution> ActiveRecurringDonations { get; set; } = [];

    public decimal TotalDonated { get; set; }

    public decimal TotalRaisedByCauses { get; set; }
}
