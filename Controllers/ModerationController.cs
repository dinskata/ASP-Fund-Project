using ASP_Fund_Project.Data;
using ASP_Fund_Project.Models;
using ASP_Fund_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Controllers;

[Authorize(Roles = ApplicationRoles.Administrator)]
public class ModerationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFundingCampaignService _fundingCampaignService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ModerationController(
        ApplicationDbContext context,
        IFundingCampaignService fundingCampaignService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _fundingCampaignService = fundingCampaignService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? userSearch, string userRole = "all")
    {
        var pendingCampaigns = await _fundingCampaignService.GetPendingApprovalAsync();
        var visibleCampaigns = await _fundingCampaignService.GetVisibleAsync(null, true);
        var deletionRequests = visibleCampaigns.Where(campaign => campaign.IsDeletionRequested).ToList();
        var beneficiaries = await _context.Beneficiaries
            .AsNoTracking()
            .Include(beneficiary => beneficiary.ManagerUser)
            .OrderBy(beneficiary => beneficiary.FullName)
            .ToListAsync();
        var allUsers = await _userManager.Users
            .Include(user => user.CreatedCampaigns)
            .OrderBy(user => user.Email)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(userSearch))
        {
            var normalizedSearch = userSearch.Trim().ToLowerInvariant();
            allUsers = allUsers
                .Where(user =>
                    user.FullName.ToLowerInvariant().Contains(normalizedSearch) ||
                    (user.Email != null && user.Email.ToLowerInvariant().Contains(normalizedSearch)) ||
                    (user.PhoneNumber != null && user.PhoneNumber.Contains(userSearch.Trim())))
                .ToList();
        }

        if (userRole != "all")
        {
            var filteredUsers = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, ApplicationRoles.Administrator);
                if ((userRole == "admins" && isAdmin) || (userRole == "users" && !isAdmin))
                {
                    filteredUsers.Add(user);
                }
            }

            allUsers = filteredUsers;
        }

        var users = allUsers;

        var latestDonations = await _context.Contributions
            .AsNoTracking()
            .Include(contribution => contribution.FundingCampaign)
            .ThenInclude(campaign => campaign!.Beneficiary)
            .OrderByDescending(contribution => contribution.DonatedOn)
            .Take(12)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TotalUsers = users.Count,
            TotalBeneficiaries = await _context.Beneficiaries.CountAsync(),
            TotalCampaigns = await _context.FundingCampaigns.CountAsync(),
            PendingCampaigns = pendingCampaigns.Count,
            TotalRaised = await _context.FundingCampaigns
                .Select(campaign => (decimal?)campaign.CurrentAmount)
                .SumAsync() ?? 0m,
            PendingApprovalCampaigns = pendingCampaigns,
            ApprovedCampaigns = visibleCampaigns.Where(campaign => campaign.IsApproved).ToList(),
            DeletionRequestCampaigns = deletionRequests,
            BeneficiaryReviewItems = beneficiaries,
            LatestDonations = latestDonations,
            Users = users
        };

        ViewBag.UserSearch = userSearch;
        ViewBag.UserRole = userRole;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var approved = await _fundingCampaignService.ApproveAsync(id);

        if (!approved)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Campaign approved and published.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFeatured(int id)
    {
        var updated = await _fundingCampaignService.ToggleFeaturedAsync(id);

        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Featured state updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCampaignHidden(int id)
    {
        var updated = await _fundingCampaignService.ToggleHiddenAsync(id);

        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Campaign visibility updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveDeletion(int id)
    {
        var deleted = await _fundingCampaignService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Campaign deletion approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDonationVisibility(int id)
    {
        var donation = await _context.Contributions.FindAsync(id);

        if (donation is null)
        {
            return NotFound();
        }

        donation.IsDonationHidden = !donation.IsDonationHidden;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = donation.IsDonationHidden
            ? "Donation hidden from public pages."
            : "Donation shown on public pages again.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBeneficiaryVerification(int id)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary is null)
        {
            return NotFound();
        }

        beneficiary.IsVerified = !beneficiary.IsVerified;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = beneficiary.IsVerified
            ? "Beneficiary verified."
            : "Beneficiary set to unverified.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return View(new EditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            IsAdministrator = await _userManager.IsInRoleAsync(user, ApplicationRoles.Administrator)
        });
    }

    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Include(entry => entry.CreatedCampaigns)
            .Include(entry => entry.ManagedBeneficiaries)
            .Include(entry => entry.Contributions)
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        ViewBag.HasCreatedCampaigns = user.CreatedCampaigns.Any();
        ViewBag.HasManagedBeneficiaries = user.ManagedBeneficiaries.Any();
        ViewBag.HasContributions = user.Contributions.Any();
        ViewBag.IsSelfDelete = _userManager.GetUserId(User) == user.Id;
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);

        if (user is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.NormalizedEmail = model.Email.ToUpperInvariant();
        user.NormalizedUserName = model.Email.ToUpperInvariant();
        user.PhoneNumber = model.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        var isCurrentlyAdmin = await _userManager.IsInRoleAsync(user, ApplicationRoles.Administrator);

        if (model.IsAdministrator && !isCurrentlyAdmin)
        {
            await _userManager.AddToRoleAsync(user, ApplicationRoles.Administrator);
        }
        else if (!model.IsAdministrator && isCurrentlyAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, ApplicationRoles.Administrator);
        }

        TempData["StatusMessage"] = "User account updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        var user = await _userManager.Users
            .Include(entry => entry.CreatedCampaigns)
            .Include(entry => entry.ManagedBeneficiaries)
            .Include(entry => entry.Contributions)
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        if (_userManager.GetUserId(User) == user.Id)
        {
            TempData["StatusMessage"] = "You cannot delete the currently signed-in admin account.";
            return RedirectToAction(nameof(Index));
        }

        if (user.CreatedCampaigns.Any())
        {
            TempData["StatusMessage"] = "This user cannot be deleted while they still own campaigns.";
            return RedirectToAction(nameof(DeleteUser), new { id });
        }

        if (user.ManagedBeneficiaries.Any())
        {
            TempData["StatusMessage"] = "This user cannot be deleted while they still manage organizations.";
            return RedirectToAction(nameof(DeleteUser), new { id });
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            TempData["StatusMessage"] = string.Join(" ", result.Errors.Select(error => error.Description));
            return RedirectToAction(nameof(DeleteUser), new { id });
        }

        TempData["StatusMessage"] = "User deleted.";
        return RedirectToAction(nameof(Index));
    }
}
