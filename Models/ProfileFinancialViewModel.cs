namespace ASP_Fund_Project.Models;

public class ProfileFinancialViewModel
{
    public bool IsAdmin { get; set; }

    public string DateRange { get; set; } = "lifetime";

    public CampaignCategory? Category { get; set; }

    public ContributionFrequency? Frequency { get; set; }

    public string StatusFilter { get; set; } = "successful";

    public decimal TotalRaised { get; set; }

    public int TotalDonations { get; set; }

    public decimal AverageDonation { get; set; }

    public IReadOnlyCollection<ProfileFinancialCauseRowViewModel> Causes { get; set; } = [];

    public IReadOnlyCollection<ProfileDonationRowViewModel> ActiveRecurringDonations { get; set; } = [];

    public IReadOnlyCollection<ProfileDonationRowViewModel> PastDonations { get; set; } = [];
}

public class ProfileFinancialCauseRowViewModel
{
    public int CampaignId { get; set; }

    public string CampaignTitle { get; set; } = string.Empty;

    public string BeneficiaryName { get; set; } = string.Empty;

    public CampaignCategory Category { get; set; }

    public decimal RaisedAmount { get; set; }

    public int DonationCount { get; set; }

    public decimal AverageDonation { get; set; }

    public DateTime? LastDonationOn { get; set; }
}

public class ProfileDonationRowViewModel
{
    public int ContributionId { get; set; }

    public int CampaignId { get; set; }

    public string CampaignTitle { get; set; } = string.Empty;

    public string DonorName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public ContributionFrequency Frequency { get; set; }

    public ContributionStatus Status { get; set; }

    public bool IsHiddenDonation { get; set; }

    public DateTime DonatedOn { get; set; }
}
