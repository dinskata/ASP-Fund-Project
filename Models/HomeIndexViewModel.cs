namespace ASP_Fund_Project.Models;

public class HomeIndexViewModel
{
    public int TotalCampaigns { get; set; }

    public int ActiveBeneficiaries { get; set; }

    public decimal TotalRaised { get; set; }

    public IReadOnlyCollection<FundingCampaign> FeaturedCampaigns { get; set; } = Array.Empty<FundingCampaign>();

    public IReadOnlyCollection<Contribution> RecentContributions { get; set; } = Array.Empty<Contribution>();
}
