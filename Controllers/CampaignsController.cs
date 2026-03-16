using ASP_Fund_Project.Data;
using ASP_Fund_Project.Models;
using ASP_Fund_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Controllers;

public class CampaignsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFundingCampaignService _fundingCampaignService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CampaignsController(
        ApplicationDbContext context,
        IFundingCampaignService fundingCampaignService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _fundingCampaignService = fundingCampaignService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchTerm, CampaignCategory? category, string status = "all")
    {
        var userId = _userManager.GetUserId(User);
        var campaigns = await _fundingCampaignService.GetVisibleAsync(userId, User.IsInRole(ApplicationRoles.Administrator));
        var filteredCampaigns = campaigns.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearch = searchTerm.Trim().ToLowerInvariant();
            filteredCampaigns = filteredCampaigns.Where(campaign =>
                campaign.Title.ToLowerInvariant().Contains(normalizedSearch) ||
                campaign.OrganizerName.ToLowerInvariant().Contains(normalizedSearch) ||
                (campaign.Beneficiary != null && campaign.Beneficiary.FullName.ToLowerInvariant().Contains(normalizedSearch)));
        }

        if (category.HasValue)
        {
            filteredCampaigns = filteredCampaigns.Where(campaign => campaign.Category == category.Value);
        }

        filteredCampaigns = status switch
        {
            "approved" => filteredCampaigns.Where(campaign => campaign.IsApproved),
            "pending" => filteredCampaigns.Where(campaign => !campaign.IsApproved),
            "featured" => filteredCampaigns.Where(campaign => campaign.IsApproved && campaign.IsFeatured),
            "mine" when userId != null => filteredCampaigns.Where(campaign => campaign.OwnerId == userId),
            _ => filteredCampaigns
        };

        return View(new CampaignIndexViewModel
        {
            SearchTerm = searchTerm,
            Category = category,
            Status = status,
            Campaigns = filteredCampaigns.ToList()
        });
    }

    public async Task<IActionResult> Details(int id, string? returnUrl = null)
    {
        var campaign = await _fundingCampaignService.GetAccessibleByIdAsync(
            id,
            _userManager.GetUserId(User),
            User.IsInRole(ApplicationRoles.Administrator));

        if (campaign is null)
        {
            return NotFound();
        }

        ViewBag.CanManage = CanManageCampaign(campaign);
        ViewBag.ReturnUrl = returnUrl;
        return View(campaign);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        await LoadBeneficiariesAsync(_userManager.GetUserId(User), User.IsInRole(ApplicationRoles.Administrator));

        return View(new CreateCampaignViewModel
        {
            CreationMode = "self",
            OrganizerName = user?.FullName ?? User.Identity?.Name ?? string.Empty,
            SelfBeneficiaryName = user?.FullName ?? string.Empty,
            SelfBankAccountName = user?.FullName ?? string.Empty,
            GoalAmount = 50m,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(45)
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCampaignViewModel model)
    {
        ValidateCampaignDates(model);
        ValidateBeneficiarySelection(model);

        if (!ModelState.IsValid)
        {
            await LoadBeneficiariesAsync(_userManager.GetUserId(User), User.IsInRole(ApplicationRoles.Administrator), model.BeneficiaryId);
            return View(model);
        }

        var beneficiaryId = model.BeneficiaryId ?? 0;

        if (model.CreationMode is "self" or "other")
        {
            var createdBeneficiary = new Beneficiary
            {
                Kind = BeneficiaryKind.Person,
                FullName = model.CreationMode == "self" ? model.SelfBeneficiaryName!.Trim() : model.AnotherBeneficiaryName!.Trim(),
                FocusArea = model.CreationMode == "self" ? model.SelfFocusArea!.Trim() : model.AnotherFocusArea!.Trim(),
                City = model.CreationMode == "self" ? model.SelfCity!.Trim() : model.AnotherCity!.Trim(),
                Story = model.CreationMode == "self" ? model.SelfStory!.Trim() : model.AnotherStory!.Trim(),
                BankAccountName = (model.CreationMode == "self" ? model.SelfBankAccountName : model.AnotherBankAccountName)!.Trim(),
                BankName = (model.CreationMode == "self" ? model.SelfBankName : model.AnotherBankName)!.Trim(),
                Iban = (model.CreationMode == "self" ? model.SelfIban : model.AnotherIban)!.Trim().ToUpperInvariant(),
                Bic = (model.CreationMode == "self" ? model.SelfBic : model.AnotherBic)!.Trim().ToUpperInvariant()
            };

            _context.Beneficiaries.Add(createdBeneficiary);
            await _context.SaveChangesAsync();
            beneficiaryId = createdBeneficiary.Id;
        }

        var campaign = new FundingCampaign
        {
            Title = model.Title,
            Category = model.Category,
            OrganizerName = model.OrganizerName,
            Summary = model.Summary,
            GoalAmount = model.GoalAmount,
            CurrentAmount = model.CurrentAmount,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            IsFeatured = false,
            BeneficiaryId = beneficiaryId,
            OwnerId = _userManager.GetUserId(User) ?? string.Empty,
            CreatedOn = DateTime.UtcNow,
            IsApproved = false
        };

        await _fundingCampaignService.CreateAsync(campaign);
        TempData["StatusMessage"] = "Campaign submitted successfully. It is waiting for admin approval before becoming public.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id, string? returnUrl = null)
    {
        var campaign = await _fundingCampaignService.GetByIdWithDetailsAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        if (!CanManageCampaign(campaign))
        {
            return Forbid();
        }

        await LoadBeneficiariesAsync(_userManager.GetUserId(User), User.IsInRole(ApplicationRoles.Administrator), campaign.BeneficiaryId);
        ViewBag.ReturnUrl = returnUrl;
        return View(campaign);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FundingCampaign campaign, string? returnUrl = null)
    {
        if (id != campaign.Id)
        {
            return BadRequest();
        }

        var existingCampaign = await _fundingCampaignService.GetByIdAsync(id);

        if (existingCampaign is null)
        {
            return NotFound();
        }

        if (!CanManageCampaign(existingCampaign))
        {
            return Forbid();
        }

        campaign.OwnerId = existingCampaign.OwnerId;
        campaign.CreatedOn = existingCampaign.CreatedOn;
        campaign.IsHidden = existingCampaign.IsHidden;
        campaign.IsDeletionRequested = existingCampaign.IsDeletionRequested;
        campaign.IsApproved = User.IsInRole(ApplicationRoles.Administrator) ? campaign.IsApproved : false;
        campaign.IsFeatured = User.IsInRole(ApplicationRoles.Administrator) ? campaign.IsFeatured : existingCampaign.IsFeatured;

        ValidateCampaignDates(campaign);

        if (!ModelState.IsValid)
        {
            await LoadBeneficiariesAsync(_userManager.GetUserId(User), User.IsInRole(ApplicationRoles.Administrator), campaign.BeneficiaryId);
            var detailedCampaign = await _fundingCampaignService.GetByIdWithDetailsAsync(campaign.Id);
            campaign.Contributions = detailedCampaign?.Contributions ?? [];
            return View(campaign);
        }

        var updated = await _fundingCampaignService.UpdateAsync(campaign);

        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = campaign.IsApproved
            ? "Campaign updated."
            : "Campaign updated and sent back to the approval queue.";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    public async Task<IActionResult> Approve(int id, string? returnUrl = null)
    {
        var campaign = await _fundingCampaignService.GetByIdWithDetailsAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(campaign);
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    [HttpPost, ActionName("Approve")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveConfirmed(int id, string? returnUrl = null)
    {
        var approved = await _fundingCampaignService.ApproveAsync(id);

        if (!approved)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Campaign approved.";
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBeneficiaryVerificationFromCampaign(int id, int beneficiaryId, string? returnAction = null)
    {
        var campaign = await _fundingCampaignService.GetByIdAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        var beneficiary = await _context.Beneficiaries.FindAsync(beneficiaryId);

        if (beneficiary is null)
        {
            return NotFound();
        }

        beneficiary.IsVerified = !beneficiary.IsVerified;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = beneficiary.IsVerified
            ? "Beneficiary verified."
            : "Beneficiary set to unverified.";

        return RedirectToAction(returnAction == "Edit" ? nameof(Edit) : nameof(Details), new { id });
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCommentVisibility(int id, int contributionId)
    {
        var campaign = await _fundingCampaignService.GetByIdAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        var contribution = await _context.Contributions
            .FirstOrDefaultAsync(item => item.Id == contributionId && item.FundingCampaignId == id);

        if (contribution is null)
        {
            return NotFound();
        }

        contribution.IsCommentHidden = !contribution.IsCommentHidden;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = contribution.IsCommentHidden
            ? "Comment hidden."
            : "Comment shown again.";

        return RedirectToAction(nameof(Edit), new { id });
    }

    public async Task<IActionResult> Donate(int id)
    {
        var campaign = await _fundingCampaignService.GetByIdWithDetailsAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        if (!campaign.IsApproved)
        {
            TempData["StatusMessage"] = "Only approved campaigns can receive donations.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var beneficiary = campaign.Beneficiary!;

        return View(new DonateViewModel
        {
            FundingCampaignId = campaign.Id,
            CampaignTitle = campaign.Title,
            BeneficiaryName = beneficiary.FullName,
            BankAccountName = beneficiary.BankAccountName,
            BankName = beneficiary.BankName,
            Iban = beneficiary.Iban,
            Bic = beneficiary.Bic,
            DonorName = User.Identity?.Name,
            Amount = 25m
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Donate(DonateViewModel model)
    {
        var campaign = await _fundingCampaignService.GetByIdWithDetailsAsync(model.FundingCampaignId);

        if (campaign is null)
        {
            return NotFound();
        }

        if (!campaign.IsApproved)
        {
            TempData["StatusMessage"] = "Only approved campaigns can receive donations.";
            return RedirectToAction(nameof(Details), new { id = model.FundingCampaignId });
        }

        model.CampaignTitle = campaign.Title;
        model.BeneficiaryName = campaign.Beneficiary?.FullName ?? string.Empty;
        model.BankAccountName = campaign.Beneficiary?.BankAccountName ?? string.Empty;
        model.BankName = campaign.Beneficiary?.BankName ?? string.Empty;
        model.Iban = campaign.Beneficiary?.Iban ?? string.Empty;
        model.Bic = campaign.Beneficiary?.Bic ?? string.Empty;

        if (!model.IsAnonymous && string.IsNullOrWhiteSpace(model.DonorName))
        {
            ModelState.AddModelError(nameof(DonateViewModel.DonorName), "Donor name is required unless the donation is anonymous.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var status = model.CardTestOutcome == "success"
            ? ContributionStatus.Successful
            : ContributionStatus.Unsuccessful;

        var contribution = new Contribution
        {
            DonorName = model.IsAnonymous ? "Anonymous donor" : model.DonorName!.Trim(),
            Amount = model.Amount,
            DonatedOn = DateTime.UtcNow,
            Note = model.Frequency == ContributionFrequency.OneTime ? model.Note : null,
            IsDonationHidden = model.IsHiddenDonation,
            PaymentMethod = ContributionPaymentMethod.Card,
            Frequency = model.Frequency,
            Status = status,
            FundingCampaignId = model.FundingCampaignId,
            ContributorUserId = _userManager.GetUserId(User)
        };

        _context.Contributions.Add(contribution);

        if (status == ContributionStatus.Successful)
        {
            var trackedCampaign = await _context.FundingCampaigns.FindAsync(model.FundingCampaignId);
            if (trackedCampaign is not null)
            {
                trackedCampaign.CurrentAmount += model.Amount;
            }
        }

        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = status switch
        {
            ContributionStatus.Successful => "SUCCESSFUL DONATION",
            ContributionStatus.Unsuccessful => "UNSUCCESSFUL DONATION",
            _ => "UNSUCCESSFUL DONATION"
        };
        TempData["DonationResult"] = status == ContributionStatus.Successful ? "success" : "failed";
        TempData["DonationId"] = contribution.Id;

        return RedirectToAction(nameof(Details), new { id = model.FundingCampaignId });
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var campaign = await _fundingCampaignService.GetByIdWithDetailsAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        if (!CanManageCampaign(campaign))
        {
            return Forbid();
        }

        ViewBag.HasDonations = campaign.Contributions.Any();

        return View(campaign);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var campaign = await _fundingCampaignService.GetByIdWithDetailsAsync(id);

        if (campaign is null)
        {
            return NotFound();
        }

        if (!CanManageCampaign(campaign))
        {
            return Forbid();
        }

        if (campaign.Contributions.Any())
        {
            var requested = await _fundingCampaignService.RequestDeletionAsync(id);

            if (!requested)
            {
                return NotFound();
            }

            TempData["StatusMessage"] = "Deletion requested. The campaign is hidden until an administrator reviews it.";
            return RedirectToAction(nameof(Index));
        }

        var deleted = await _fundingCampaignService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Campaign deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadBeneficiariesAsync(string? userId, bool isAdministrator, int? selectedId = null)
    {
        var query = _context.Beneficiaries
            .AsNoTracking()
            .AsQueryable();

        if (isAdministrator)
        {
            query = query
                .Where(beneficiary => beneficiary.IsVerified);
        }
        else
        {
            query = query.Where(beneficiary =>
                beneficiary.IsVerified &&
                beneficiary.Kind == BeneficiaryKind.Organization &&
                beneficiary.ManagerUserId == userId);
        }

        ViewBag.BeneficiaryId = new SelectList(
            await query
                .OrderBy(beneficiary => beneficiary.FullName)
                .ToListAsync(),
            nameof(Beneficiary.Id),
            nameof(Beneficiary.FullName),
            selectedId);
    }

    private void ValidateCampaignDates(CreateCampaignViewModel campaign)
    {
        if (campaign.EndDate < campaign.StartDate)
        {
            ModelState.AddModelError(nameof(CreateCampaignViewModel.EndDate), "End date must be after the start date.");
        }

        if (campaign.CurrentAmount > campaign.GoalAmount)
        {
            ModelState.AddModelError(nameof(CreateCampaignViewModel.CurrentAmount), "Current amount cannot be higher than the goal amount.");
        }
    }

    private void ValidateCampaignDates(FundingCampaign campaign)
    {
        if (campaign.EndDate < campaign.StartDate)
        {
            ModelState.AddModelError(nameof(FundingCampaign.EndDate), "End date must be after the start date.");
        }

        if (campaign.CurrentAmount > campaign.GoalAmount)
        {
            ModelState.AddModelError(nameof(FundingCampaign.CurrentAmount), "Current amount cannot be higher than the goal amount.");
        }
    }

    private void ValidateBeneficiarySelection(CreateCampaignViewModel model)
    {
        if (model.CreationMode is not ("self" or "other" or "approved"))
        {
            ModelState.AddModelError(nameof(CreateCampaignViewModel.CreationMode), "Choose how you want to create the cause.");
            return;
        }

        if (model.CreationMode == "approved")
        {
            if (!model.BeneficiaryId.HasValue)
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.BeneficiaryId), "Select one of your approved organizations.");
                return;
            }

            var userId = _userManager.GetUserId(User);
            var selectedBeneficiary = _context.Beneficiaries
                .AsNoTracking()
                .FirstOrDefault(beneficiary => beneficiary.Id == model.BeneficiaryId.Value);

            if (selectedBeneficiary is null || !selectedBeneficiary.IsVerified || selectedBeneficiary.Kind != BeneficiaryKind.Organization)
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.BeneficiaryId), "Select one of your approved organizations.");
                return;
            }

            if (!User.IsInRole(ApplicationRoles.Administrator) && selectedBeneficiary.ManagerUserId != userId)
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.BeneficiaryId), "You can only choose your own approved organizations.");
                return;
            }
        }

        if (model.CreationMode == "self")
        {
            if (string.IsNullOrWhiteSpace(model.SelfBeneficiaryName))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.SelfBeneficiaryName), "Your name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelfFocusArea))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.SelfFocusArea), "Reason for support is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelfCity))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.SelfCity), "Town/City is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelfStory))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.SelfStory), "Beneficiary story/information is required.");
            }

            ValidateBankDetails(
                model.SelfBankAccountName,
                model.SelfBankName,
                model.SelfIban,
                model.SelfBic,
                nameof(CreateCampaignViewModel.SelfBankAccountName),
                nameof(CreateCampaignViewModel.SelfBankName),
                nameof(CreateCampaignViewModel.SelfIban),
                nameof(CreateCampaignViewModel.SelfBic));
        }
        else if (model.CreationMode == "other")
        {
            if (string.IsNullOrWhiteSpace(model.AnotherBeneficiaryName))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.AnotherBeneficiaryName), "Beneficiary name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.AnotherFocusArea))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.AnotherFocusArea), "Reason for support is required.");
            }

            if (string.IsNullOrWhiteSpace(model.AnotherCity))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.AnotherCity), "Town/City is required.");
            }

            if (string.IsNullOrWhiteSpace(model.AnotherStory))
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.AnotherStory), "Beneficiary story/information is required.");
            }

            ValidateBankDetails(
                model.AnotherBankAccountName,
                model.AnotherBankName,
                model.AnotherIban,
                model.AnotherBic,
                nameof(CreateCampaignViewModel.AnotherBankAccountName),
                nameof(CreateCampaignViewModel.AnotherBankName),
                nameof(CreateCampaignViewModel.AnotherIban),
                nameof(CreateCampaignViewModel.AnotherBic));
        }
        else if (User.IsInRole(ApplicationRoles.Administrator))
        {
            if (!model.BeneficiaryId.HasValue)
            {
                ModelState.AddModelError(nameof(CreateCampaignViewModel.BeneficiaryId), "Select a verified beneficiary.");
            }
        }
    }

    private bool CanManageCampaign(FundingCampaign campaign)
    {
        var userId = _userManager.GetUserId(User);
        return User.IsInRole(ApplicationRoles.Administrator) || campaign.OwnerId == userId;
    }

    private void ValidateBankDetails(
        string? accountHolder,
        string? bankName,
        string? iban,
        string? bic,
        string accountHolderField,
        string bankNameField,
        string ibanField,
        string bicField)
    {
        if (string.IsNullOrWhiteSpace(accountHolder))
        {
            ModelState.AddModelError(accountHolderField, "Account holder is required.");
        }

        if (string.IsNullOrWhiteSpace(bankName))
        {
            ModelState.AddModelError(bankNameField, "Bank name is required.");
        }

        if (string.IsNullOrWhiteSpace(iban))
        {
            ModelState.AddModelError(ibanField, "IBAN is required.");
        }

        if (string.IsNullOrWhiteSpace(bic))
        {
            ModelState.AddModelError(bicField, "BIC is required.");
        }
    }
}
