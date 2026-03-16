namespace ASP_Fund_Project.Models;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }

    public int TotalBeneficiaries { get; set; }

    public int TotalCampaigns { get; set; }

    public int PendingCampaigns { get; set; }

    public decimal TotalRaised { get; set; }

    public IReadOnlyCollection<FundingCampaign> PendingApprovalCampaigns { get; set; } = Array.Empty<FundingCampaign>();

    public IReadOnlyCollection<FundingCampaign> ApprovedCampaigns { get; set; } = Array.Empty<FundingCampaign>();

    public IReadOnlyCollection<FundingCampaign> DeletionRequestCampaigns { get; set; } = Array.Empty<FundingCampaign>();

    public IReadOnlyCollection<Beneficiary> BeneficiaryReviewItems { get; set; } = Array.Empty<Beneficiary>();

    public IReadOnlyCollection<Contribution> LatestDonations { get; set; } = Array.Empty<Contribution>();

    public IReadOnlyCollection<ApplicationUser> Users { get; set; } = Array.Empty<ApplicationUser>();
}
