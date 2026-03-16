using System.Diagnostics;
using ASP_Fund_Project.Data;
using ASP_Fund_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredCampaigns = await _context.FundingCampaigns
            .AsNoTracking()
            .Include(campaign => campaign.Beneficiary)
            .Where(campaign => campaign.IsFeatured && campaign.IsApproved)
            .OrderBy(campaign => campaign.EndDate)
            .Take(3)
            .ToListAsync();

        var recentContributions = await _context.Contributions
            .AsNoTracking()
            .Include(contribution => contribution.FundingCampaign)
            .Where(contribution => contribution.FundingCampaign != null &&
                                   contribution.FundingCampaign.IsApproved &&
                                   !contribution.IsDonationHidden)
            .OrderByDescending(contribution => contribution.DonatedOn)
            .ThenByDescending(contribution => contribution.Id)
            .Take(4)
            .ToListAsync();

        var model = new HomeIndexViewModel
        {
            TotalCampaigns = await _context.FundingCampaigns.CountAsync(campaign => campaign.IsApproved),
            ActiveBeneficiaries = await _context.Beneficiaries.CountAsync(),
            TotalRaised = await _context.FundingCampaigns
                .Where(campaign => campaign.IsApproved)
                .Select(campaign => (decimal?)campaign.CurrentAmount)
                .SumAsync() ?? 0m,
            FeaturedCampaigns = featuredCampaigns,
            RecentContributions = recentContributions
        };

        return View(model);
    }

    public IActionResult Support()
    {
        return View(new SupportInquiryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Support(SupportInquiryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TempData["StatusMessage"] = "Demo only. Your inquiry was not actually sent.";
        return RedirectToAction(nameof(Support));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
