using ASP_Fund_Project.Models;

namespace ASP_Fund_Project.Services;

public interface IFundingCampaignService
{
    Task<IReadOnlyCollection<FundingCampaign>> GetVisibleAsync(string? userId, bool isAdministrator);

    Task<IReadOnlyCollection<FundingCampaign>> GetPendingApprovalAsync();

    Task<FundingCampaign?> GetByIdAsync(int id);

    Task<FundingCampaign?> GetByIdWithDetailsAsync(int id);

    Task<FundingCampaign?> GetAccessibleByIdAsync(int id, string? userId, bool isAdministrator);

    Task CreateAsync(FundingCampaign campaign);

    Task<bool> UpdateAsync(FundingCampaign campaign);

    Task<bool> ApproveAsync(int id);

    Task<bool> ToggleFeaturedAsync(int id);

    Task<bool> RequestDeletionAsync(int id);

    Task<bool> ToggleHiddenAsync(int id);

    Task<bool> DeleteAsync(int id);
}
