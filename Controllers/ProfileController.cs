using ASP_Fund_Project.Models;
using ASP_Fund_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Settings));
    }

    public async Task<IActionResult> Settings()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        return View(BuildSettingsViewModel(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(EditProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        if (user.Id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var wantsPasswordChange =
            !string.IsNullOrWhiteSpace(model.CurrentPassword) ||
            !string.IsNullOrWhiteSpace(model.NewPassword) ||
            !string.IsNullOrWhiteSpace(model.ConfirmNewPassword);

        if (wantsPasswordChange)
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError(nameof(EditProfileViewModel.CurrentPassword), "Current password is required.");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError(nameof(EditProfileViewModel.NewPassword), "New password is required.");
            }

            if (string.IsNullOrWhiteSpace(model.ConfirmNewPassword))
            {
                ModelState.AddModelError(nameof(EditProfileViewModel.ConfirmNewPassword), "Confirm the new password.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
        }

        user.FullName = model.FullName.Trim();
        user.Email = model.Email.Trim();
        user.UserName = model.Email.Trim();
        user.NormalizedEmail = model.Email.Trim().ToUpperInvariant();
        user.NormalizedUserName = model.Email.Trim().ToUpperInvariant();
        user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        if (wantsPasswordChange)
        {
            var passwordResult = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword!,
                model.NewPassword!);

            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = wantsPasswordChange
            ? "Profile and password updated."
            : "Profile updated.";
        return RedirectToAction(nameof(Settings));
    }

    public async Task<IActionResult> Organizations(string? search, string status = "all")
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        var isAdmin = User.IsInRole(ApplicationRoles.Administrator);
        var query = _context.Beneficiaries
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(beneficiary => beneficiary.ManagerUserId == user.Id);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();
            query = query.Where(beneficiary =>
                beneficiary.FullName.ToLower().Contains(normalizedSearch) ||
                beneficiary.FocusArea.ToLower().Contains(normalizedSearch) ||
                beneficiary.City.ToLower().Contains(normalizedSearch));
        }

        query = status switch
        {
            "verified" => query.Where(beneficiary => beneficiary.IsVerified),
            "unverified" => query.Where(beneficiary => !beneficiary.IsVerified),
            _ => query
        };

        var organizations = await query
            .OrderBy(beneficiary => beneficiary.FullName)
            .ToListAsync();

        ViewBag.IsAdminProfile = isAdmin;
        ViewBag.Search = search;
        ViewBag.Status = status;

        return View(organizations);
    }

    public async Task<IActionResult> Causes(string? search, string status = "all", CampaignCategory? category = null)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        var isAdmin = User.IsInRole(ApplicationRoles.Administrator);
        var query = _context.FundingCampaigns
            .AsNoTracking()
            .Include(campaign => campaign.Beneficiary)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(campaign => campaign.OwnerId == user.Id);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();
            query = query.Where(campaign =>
                campaign.Title.ToLower().Contains(normalizedSearch) ||
                campaign.OrganizerName.ToLower().Contains(normalizedSearch) ||
                campaign.Beneficiary!.FullName.ToLower().Contains(normalizedSearch));
        }

        if (category.HasValue)
        {
            query = query.Where(campaign => campaign.Category == category.Value);
        }

        query = status switch
        {
            "approved" => query.Where(campaign => campaign.IsApproved),
            "pending" => query.Where(campaign => !campaign.IsApproved),
            "featured" => query.Where(campaign => campaign.IsFeatured),
            _ => query
        };

        var causes = await query
            .OrderByDescending(campaign => campaign.CreatedOn)
            .ToListAsync();

        ViewBag.IsAdminProfile = isAdmin;
        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.Category = category;

        return View(causes);
    }

    public async Task<IActionResult> MyDonations(string filter = "all")
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        var query = _context.Contributions
            .AsNoTracking()
            .Include(contribution => contribution.FundingCampaign)
            .Where(contribution => contribution.ContributorUserId == user.Id)
            .AsQueryable();

        query = filter switch
        {
            "active" => query.Where(contribution =>
                contribution.Frequency != ContributionFrequency.OneTime &&
                contribution.Status == ContributionStatus.Successful),
            "recurring" => query.Where(contribution => contribution.Frequency != ContributionFrequency.OneTime),
            "hidden" => query.Where(contribution => contribution.IsDonationHidden),
            _ => query
        };

        var donations = await query
            .OrderByDescending(contribution => contribution.DonatedOn)
            .ThenByDescending(contribution => contribution.Id)
            .ToListAsync();

        ViewBag.Filter = filter;
        return View(donations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDonationVisibility(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        var donation = await _context.Contributions
            .FirstOrDefaultAsync(contribution => contribution.Id == id && contribution.ContributorUserId == user.Id);

        if (donation is null)
        {
            return NotFound();
        }

        donation.IsDonationHidden = !donation.IsDonationHidden;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = donation.IsDonationHidden
            ? "Donation hidden."
            : "Donation visible again.";

        return RedirectToAction(nameof(MyDonations));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelRecurringDonation(int id, CampaignCategory? category, ContributionFrequency? frequency, string status = "successful", string dateRange = "lifetime")
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        var isAdmin = User.IsInRole(ApplicationRoles.Administrator);
        var donation = await _context.Contributions
            .Include(contribution => contribution.FundingCampaign)
            .FirstOrDefaultAsync(contribution => contribution.Id == id);

        if (donation is null)
        {
            return NotFound();
        }

        if (!isAdmin && donation.ContributorUserId != user.Id)
        {
            return Forbid();
        }

        if (donation.Frequency == ContributionFrequency.OneTime)
        {
            TempData["StatusMessage"] = "Only weekly or monthly donations can be cancelled.";
            return isAdmin
                ? RedirectToAction(nameof(Financial), new { category, frequency, status, dateRange })
                : RedirectToAction(nameof(MyDonations));
        }

        donation.Status = ContributionStatus.Cancelled;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Recurring donation cancelled.";
        return isAdmin
            ? RedirectToAction(nameof(Financial), new { category, frequency, status, dateRange })
            : RedirectToAction(nameof(MyDonations));
    }

    public async Task<IActionResult> Financial(CampaignCategory? category, ContributionFrequency? frequency, string status = "successful", string dateRange = "lifetime")
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        var isAdmin = User.IsInRole(ApplicationRoles.Administrator);
        var now = DateTime.UtcNow;
        DateTime? fromDate = dateRange switch
        {
            "last30days" => now.AddDays(-30),
            "thisyear" => new DateTime(now.Year, 1, 1),
            _ => null
        };

        var campaigns = await _context.FundingCampaigns
            .AsNoTracking()
            .Include(campaign => campaign.Beneficiary)
            .Include(campaign => campaign.Contributions)
            .Where(campaign => isAdmin || campaign.OwnerId == user.Id)
            .Where(campaign => !category.HasValue || campaign.Category == category.Value)
            .ToListAsync();

        IEnumerable<Contribution> ApplyContributionFilters(IEnumerable<Contribution> contributions)
        {
            var filtered = contributions;

            if (fromDate.HasValue)
            {
                filtered = filtered.Where(contribution => contribution.DonatedOn >= fromDate.Value);
            }

            if (frequency.HasValue)
            {
                filtered = filtered.Where(contribution => contribution.Frequency == frequency.Value);
            }

            filtered = status switch
            {
                "all" => filtered,
                "unsuccessful" => filtered.Where(contribution => contribution.Status == ContributionStatus.Unsuccessful),
                "pending" => filtered.Where(contribution => contribution.Status == ContributionStatus.PendingBankTransfer),
                "cancelled" => filtered.Where(contribution => contribution.Status == ContributionStatus.Cancelled),
                _ => filtered.Where(contribution => contribution.Status == ContributionStatus.Successful)
            };

            return filtered;
        }

        var causeRows = campaigns
            .Select(campaign =>
            {
                var filteredContributions = ApplyContributionFilters(campaign.Contributions).ToList();

                return new ProfileFinancialCauseRowViewModel
                {
                    CampaignId = campaign.Id,
                    CampaignTitle = campaign.Title,
                    BeneficiaryName = campaign.Beneficiary?.FullName ?? string.Empty,
                    Category = campaign.Category,
                    RaisedAmount = filteredContributions.Sum(contribution => contribution.Amount),
                    DonationCount = filteredContributions.Count,
                    AverageDonation = filteredContributions.Count == 0
                        ? 0
                        : filteredContributions.Average(contribution => contribution.Amount),
                    LastDonationOn = filteredContributions
                        .OrderByDescending(contribution => contribution.DonatedOn)
                        .Select(contribution => (DateTime?)contribution.DonatedOn)
                        .FirstOrDefault()
                };
            })
            .OrderByDescending(row => row.RaisedAmount)
            .ThenBy(row => row.CampaignTitle)
            .ToList();

        var allFilteredContributions = causeRows
            .SelectMany(row => campaigns
                .First(campaign => campaign.Id == row.CampaignId)
                .Contributions)
            .ToList();

        var filteredTotals = ApplyContributionFilters(allFilteredContributions).ToList();
        var recurringDonations = filteredTotals
            .Where(contribution =>
                contribution.Status == ContributionStatus.Successful &&
                contribution.Frequency != ContributionFrequency.OneTime)
            .OrderByDescending(contribution => contribution.DonatedOn)
            .ThenByDescending(contribution => contribution.Id)
            .Select(contribution => new ProfileDonationRowViewModel
            {
                ContributionId = contribution.Id,
                CampaignId = contribution.FundingCampaignId,
                CampaignTitle = campaigns.First(campaign => campaign.Id == contribution.FundingCampaignId).Title,
                DonorName = contribution.DonorName,
                Amount = contribution.Amount,
                Frequency = contribution.Frequency,
                Status = contribution.Status,
                IsHiddenDonation = contribution.IsDonationHidden,
                DonatedOn = contribution.DonatedOn
            })
            .ToList();

        var pastDonations = filteredTotals
            .OrderByDescending(contribution => contribution.DonatedOn)
            .ThenByDescending(contribution => contribution.Id)
            .Select(contribution => new ProfileDonationRowViewModel
            {
                ContributionId = contribution.Id,
                CampaignId = contribution.FundingCampaignId,
                CampaignTitle = campaigns.First(campaign => campaign.Id == contribution.FundingCampaignId).Title,
                DonorName = contribution.DonorName,
                Amount = contribution.Amount,
                Frequency = contribution.Frequency,
                Status = contribution.Status,
                IsHiddenDonation = contribution.IsDonationHidden,
                DonatedOn = contribution.DonatedOn
            })
            .ToList();

        return View(new ProfileFinancialViewModel
        {
            IsAdmin = isAdmin,
            DateRange = dateRange,
            Category = category,
            Frequency = frequency,
            StatusFilter = status,
            TotalRaised = filteredTotals.Sum(contribution => contribution.Amount),
            TotalDonations = filteredTotals.Count,
            AverageDonation = filteredTotals.Count == 0 ? 0 : filteredTotals.Average(contribution => contribution.Amount),
            Causes = causeRows,
            ActiveRecurringDonations = recurringDonations,
            PastDonations = pastDonations
        });
    }

    private static EditProfileViewModel BuildSettingsViewModel(ApplicationUser user)
    {
        return new EditProfileViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber
        };
    }
}
