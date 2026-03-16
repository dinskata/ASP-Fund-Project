using ASP_Fund_Project.Data;
using ASP_Fund_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Services;

public class FundingCampaignService : IFundingCampaignService
{
    private readonly ApplicationDbContext _context;

    public FundingCampaignService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<FundingCampaign>> GetVisibleAsync(string? userId, bool isAdministrator)
    {
        var query = _context.FundingCampaigns
            .AsNoTracking()
            .Include(c => c.Beneficiary)
            .Include(c => c.Owner)
            .AsQueryable();

        if (!isAdministrator)
        {
            query = query.Where(c => (!c.IsHidden && c.IsApproved) || (userId != null && c.OwnerId == userId));
        }

        return await query
            .OrderByDescending(c => c.IsApproved)
            .ThenByDescending(c => c.IsFeatured)
            .ThenBy(c => c.EndDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<FundingCampaign>> GetPendingApprovalAsync()
    {
        return await _context.FundingCampaigns
            .AsNoTracking()
            .Include(c => c.Beneficiary)
            .Include(c => c.Owner)
            .Where(c => !c.IsApproved)
            .OrderByDescending(c => c.CreatedOn)
            .ToListAsync();
    }

    public async Task<FundingCampaign?> GetByIdAsync(int id)
    {
        return await _context.FundingCampaigns
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<FundingCampaign?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.FundingCampaigns
            .AsNoTracking()
            .Include(c => c.Beneficiary)
            .Include(c => c.Owner)
            .Include(c => c.Contributions.OrderByDescending(contribution => contribution.DonatedOn).ThenByDescending(contribution => contribution.Id))
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<FundingCampaign?> GetAccessibleByIdAsync(int id, string? userId, bool isAdministrator)
    {
        var campaign = await GetByIdWithDetailsAsync(id);

        if (campaign is null)
        {
            return null;
        }

        if ((!campaign.IsHidden && campaign.IsApproved) || isAdministrator || (userId != null && campaign.OwnerId == userId))
        {
            return campaign;
        }

        return null;
    }

    public async Task CreateAsync(FundingCampaign campaign)
    {
        _context.FundingCampaigns.Add(campaign);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(FundingCampaign campaign)
    {
        var existingCampaign = await _context.FundingCampaigns.FindAsync(campaign.Id);

        if (existingCampaign is null)
        {
            return false;
        }

        existingCampaign.Title = campaign.Title;
        existingCampaign.Category = campaign.Category;
        existingCampaign.OrganizerName = campaign.OrganizerName;
        existingCampaign.Summary = campaign.Summary;
        existingCampaign.GoalAmount = campaign.GoalAmount;
        existingCampaign.CurrentAmount = campaign.CurrentAmount;
        existingCampaign.StartDate = campaign.StartDate;
        existingCampaign.EndDate = campaign.EndDate;
        existingCampaign.IsFeatured = campaign.IsFeatured;
        existingCampaign.BeneficiaryId = campaign.BeneficiaryId;
        existingCampaign.IsApproved = campaign.IsApproved;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAsync(int id)
    {
        var campaign = await _context.FundingCampaigns.FindAsync(id);

        if (campaign is null)
        {
            return false;
        }

        campaign.IsApproved = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleFeaturedAsync(int id)
    {
        var campaign = await _context.FundingCampaigns.FindAsync(id);

        if (campaign is null || !campaign.IsApproved)
        {
            return false;
        }

        campaign.IsFeatured = !campaign.IsFeatured;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingCampaign = await _context.FundingCampaigns.FindAsync(id);

        if (existingCampaign is null)
        {
            return false;
        }

        _context.FundingCampaigns.Remove(existingCampaign);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestDeletionAsync(int id)
    {
        var campaign = await _context.FundingCampaigns.FindAsync(id);

        if (campaign is null)
        {
            return false;
        }

        campaign.IsDeletionRequested = true;
        campaign.IsHidden = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleHiddenAsync(int id)
    {
        var campaign = await _context.FundingCampaigns.FindAsync(id);

        if (campaign is null)
        {
            return false;
        }

        campaign.IsHidden = !campaign.IsHidden;
        await _context.SaveChangesAsync();
        return true;
    }
}
