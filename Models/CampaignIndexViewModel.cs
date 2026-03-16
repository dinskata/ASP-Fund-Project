namespace ASP_Fund_Project.Models;

public class CampaignIndexViewModel
{
    public string? SearchTerm { get; set; }

    public CampaignCategory? Category { get; set; }

    public string Status { get; set; } = "all";

    public IReadOnlyCollection<FundingCampaign> Campaigns { get; set; } = [];
}
