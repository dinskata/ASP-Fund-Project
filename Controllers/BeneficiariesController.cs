using ASP_Fund_Project.Data;
using ASP_Fund_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Controllers;

public class BeneficiariesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BeneficiariesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var beneficiaries = await _context.Beneficiaries
            .AsNoTracking()
            .Include(beneficiary => beneficiary.FundingCampaigns)
            .OrderBy(beneficiary => beneficiary.FullName)
            .ToListAsync();

        return View(beneficiaries);
    }

    public async Task<IActionResult> Details(int id, string? returnUrl = null)
    {
        var beneficiary = await _context.Beneficiaries
            .AsNoTracking()
            .Include(entry => entry.FundingCampaigns.OrderBy(campaign => campaign.EndDate))
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (beneficiary is null)
        {
            return NotFound();
        }

        ViewBag.CanEditBeneficiary = CanEditBeneficiary(beneficiary);
        ViewBag.ReturnUrl = returnUrl;
        return View(beneficiary);
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    public IActionResult Create()
    {
        return View(new Beneficiary());
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Beneficiary beneficiary)
    {
        if (!ModelState.IsValid)
        {
            return View(beneficiary);
        }

        _context.Beneficiaries.Add(beneficiary);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Beneficiary added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> RegisterOrganization()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        return View(new OrganizationRegistrationViewModel
        {
            BankAccountName = user.FullName
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterOrganization(OrganizationRegistrationViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var beneficiary = new Beneficiary
        {
            Kind = BeneficiaryKind.Organization,
            FullName = model.FullName.Trim(),
            FocusArea = model.FocusArea.Trim(),
            City = model.City.Trim(),
            Story = model.Story.Trim(),
            BankAccountName = model.BankAccountName.Trim(),
            BankName = model.BankName.Trim(),
            Iban = model.Iban.Trim().ToUpperInvariant(),
            Bic = model.Bic.Trim().ToUpperInvariant(),
            ManagerUserId = user.Id
        };

        _context.Beneficiaries.Add(beneficiary);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Organization beneficiary registered.";
        return RedirectToAction(nameof(Details), new { id = beneficiary.Id });
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id, string? returnUrl = null)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary is null)
        {
            return NotFound();
        }

        if (!CanEditBeneficiary(beneficiary))
        {
            return Forbid();
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(beneficiary);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Beneficiary beneficiary, string? returnUrl = null)
    {
        if (id != beneficiary.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(beneficiary);
        }

        var existingBeneficiary = await _context.Beneficiaries
            .AsNoTracking()
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (existingBeneficiary is null)
        {
            return NotFound();
        }

        if (!CanEditBeneficiary(existingBeneficiary))
        {
            return Forbid();
        }

        if (!User.IsInRole(ApplicationRoles.Administrator))
        {
            beneficiary.ManagerUserId = existingBeneficiary.ManagerUserId;
            beneficiary.Kind = existingBeneficiary.Kind;
        }

        beneficiary.Iban = beneficiary.Iban.Trim().ToUpperInvariant();
        beneficiary.Bic = beneficiary.Bic.Trim().ToUpperInvariant();

        _context.Update(beneficiary);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Beneficiary updated successfully.";
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    public async Task<IActionResult> Verify(int id, string? returnUrl = null)
    {
        var beneficiary = await _context.Beneficiaries
            .AsNoTracking()
            .Include(entry => entry.ManagerUser)
            .Include(entry => entry.FundingCampaigns)
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (beneficiary is null)
        {
            return NotFound();
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(beneficiary);
    }

    [Authorize(Roles = ApplicationRoles.Administrator)]
    [HttpPost, ActionName("Verify")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyConfirmed(int id, string? returnUrl = null)
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

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var beneficiary = await _context.Beneficiaries
            .AsNoTracking()
            .Include(entry => entry.FundingCampaigns)
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (beneficiary is null)
        {
            return NotFound();
        }

        if (!CanEditBeneficiary(beneficiary))
        {
            return Forbid();
        }

        ViewBag.HasCampaigns = beneficiary.FundingCampaigns.Any();
        return View(beneficiary);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var beneficiary = await _context.Beneficiaries
            .Include(entry => entry.FundingCampaigns)
            .FirstOrDefaultAsync(entry => entry.Id == id);

        if (beneficiary is null)
        {
            return NotFound();
        }

        if (!CanEditBeneficiary(beneficiary))
        {
            return Forbid();
        }

        if (beneficiary.FundingCampaigns.Any())
        {
            TempData["StatusMessage"] = "This organization cannot be deleted while campaigns are still linked to it. Delete the campaigns first.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _context.Beneficiaries.Remove(beneficiary);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Beneficiary deleted.";
        return RedirectToAction(nameof(Index));
    }

    private bool CanEditBeneficiary(Beneficiary beneficiary)
    {
        var userId = _userManager.GetUserId(User);
        return User.IsInRole(ApplicationRoles.Administrator) || beneficiary.ManagerUserId == userId;
    }
}
