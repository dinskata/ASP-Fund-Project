using ASP_Fund_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_Fund_Project.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();

    public DbSet<FundingCampaign> FundingCampaigns => Set<FundingCampaign>();

    public DbSet<Contribution> Contributions => Set<Contribution>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        const string adminRoleId = "af6f1f0d-5482-45c4-84dc-8d4c7ec9d001";
        const string adminUserId = "8cf2c7fc-3277-4bff-bc66-14f05fc4a001";

        builder.Entity<FundingCampaign>()
            .HasOne(campaign => campaign.Beneficiary)
            .WithMany(beneficiary => beneficiary.FundingCampaigns)
            .HasForeignKey(campaign => campaign.BeneficiaryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FundingCampaign>()
            .HasOne(campaign => campaign.Owner)
            .WithMany(owner => owner.CreatedCampaigns)
            .HasForeignKey(campaign => campaign.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Beneficiary>()
            .HasOne(beneficiary => beneficiary.ManagerUser)
            .WithMany(user => user.ManagedBeneficiaries)
            .HasForeignKey(beneficiary => beneficiary.ManagerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Contribution>()
            .HasOne(contribution => contribution.FundingCampaign)
            .WithMany(campaign => campaign.Contributions)
            .HasForeignKey(contribution => contribution.FundingCampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Contribution>()
            .HasOne(contribution => contribution.ContributorUser)
            .WithMany(user => user.Contributions)
            .HasForeignKey(contribution => contribution.ContributorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        var adminRole = new IdentityRole
        {
            Id = adminRoleId,
            Name = ApplicationRoles.Administrator,
            NormalizedName = ApplicationRoles.Administrator.ToUpperInvariant()
        };

        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin@communityfundhub.local",
            NormalizedUserName = "ADMIN@COMMUNITYFUNDHUB.LOCAL",
            Email = "admin@communityfundhub.local",
            NormalizedEmail = "ADMIN@COMMUNITYFUNDHUB.LOCAL",
            EmailConfirmed = true,
            FullName = "Platform Administrator",
            SecurityStamp = adminUserId,
            ConcurrencyStamp = adminUserId
        };

        adminUser.PasswordHash = "AQAAAAIAAYagAAAAEFxuRvZxQm/vyVMytjxyb8RkvTTy6FwpftnnlpIAqG0SupBVNaciShn7W9j4//M+jA==";

        builder.Entity<IdentityRole>().HasData(adminRole);
        builder.Entity<ApplicationUser>().HasData(adminUser);
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = adminRoleId,
            UserId = adminUserId
        });

        builder.Entity<Beneficiary>().HasData(
            new Beneficiary
            {
                Id = 1,
                Kind = BeneficiaryKind.Organization,
                FullName = "Bright Path Youth Center",
                FocusArea = "After-school education",
                City = "Sofia",
                Story = "Bright Path supports teenagers from underserved neighborhoods with tutoring, mentoring, and digital literacy workshops throughout the school year.",
                BankAccountName = "Bright Path Youth Center",
                BankName = "UniCredit Bulbank",
                Iban = "BG80UNCR70001512345678",
                Bic = "UNCRBGSF"
            },
            new Beneficiary
            {
                Id = 2,
                Kind = BeneficiaryKind.Organization,
                FullName = "St. Marina Health Circle",
                FocusArea = "Community health",
                City = "Plovdiv",
                Story = "St. Marina Health Circle organizes mobile screenings, medicine support, and care coordination for elderly residents who struggle to access regular treatment.",
                BankAccountName = "St. Marina Health Circle",
                BankName = "DSK Bank",
                Iban = "BG48STSA93000012345678",
                Bic = "STSABGSF"
            },
            new Beneficiary
            {
                Id = 3,
                Kind = BeneficiaryKind.Organization,
                FullName = "Riverbank Arts Lab",
                FocusArea = "Creative inclusion",
                City = "Varna",
                Story = "Riverbank Arts Lab runs free arts residencies and community events that help young creators turn talent into practical opportunities.",
                BankAccountName = "Riverbank Arts Lab",
                BankName = "Postbank",
                Iban = "BG30BPBI79401234567890",
                Bic = "BPBIBGSF"
            });

        builder.Entity<FundingCampaign>().HasData(
            new FundingCampaign
            {
                Id = 1,
                Title = "Laptop Library for Future Coders",
                Category = CampaignCategory.Education,
                OrganizerName = "Mila Petrova",
                Summary = "A focused campaign to equip Bright Path students with refurbished laptops and internet vouchers for project-based learning.",
                GoalAmount = 12000m,
                CurrentAmount = 8450m,
                StartDate = new DateTime(2026, 2, 10),
                EndDate = new DateTime(2026, 4, 20),
                IsFeatured = true,
                IsApproved = true,
                CreatedOn = new DateTime(2026, 2, 10),
                BeneficiaryId = 1,
                OwnerId = adminUserId
            },
            new FundingCampaign
            {
                Id = 2,
                Title = "Mobile Screenings for Seniors",
                Category = CampaignCategory.Health,
                OrganizerName = "Dragan Iliev",
                Summary = "Funding one season of neighborhood screenings, blood pressure kits, and medication tracking support for homebound seniors.",
                GoalAmount = 18000m,
                CurrentAmount = 11200m,
                StartDate = new DateTime(2026, 1, 25),
                EndDate = new DateTime(2026, 5, 15),
                IsFeatured = true,
                IsApproved = true,
                CreatedOn = new DateTime(2026, 1, 25),
                BeneficiaryId = 2,
                OwnerId = adminUserId
            },
            new FundingCampaign
            {
                Id = 3,
                Title = "Open Studio Week for Young Artists",
                Category = CampaignCategory.Arts,
                OrganizerName = "Elena Georgieva",
                Summary = "A public-facing arts week that gives emerging creators materials, exhibition space, and mentoring from local professionals.",
                GoalAmount = 9000m,
                CurrentAmount = 3900m,
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 6, 1),
                IsFeatured = false,
                IsApproved = true,
                CreatedOn = new DateTime(2026, 3, 1),
                BeneficiaryId = 3,
                OwnerId = adminUserId
            });

        builder.Entity<Contribution>().HasData(
            new Contribution
            {
                Id = 1,
                DonorName = "Ivan Kolev",
                Amount = 250m,
                DonatedOn = new DateTime(2026, 3, 11),
                Note = "Happy to support practical tech access.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 1
            },
            new Contribution
            {
                Id = 2,
                DonorName = "Nadya Hristova",
                Amount = 500m,
                DonatedOn = new DateTime(2026, 3, 12),
                Note = "For the mobile health visits.",
                PaymentMethod = ContributionPaymentMethod.BankTransfer,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.PendingBankTransfer,
                FundingCampaignId = 2
            },
            new Contribution
            {
                Id = 3,
                DonorName = "Petar Nikolov",
                Amount = 120m,
                DonatedOn = new DateTime(2026, 3, 13),
                Note = "Excited to visit the exhibition.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Monthly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 3
            },
            new Contribution
            {
                Id = 4,
                DonorName = "Teodora Yaneva",
                Amount = 320m,
                DonatedOn = new DateTime(2026, 3, 14),
                Note = "Education changes trajectories.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Weekly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 1
            },
            new Contribution
            {
                Id = 5,
                DonorName = "Kalin Georgiev",
                Amount = 85m,
                DonatedOn = new DateTime(2026, 3, 15),
                Note = "Wishing the students a strong spring term.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 1
            },
            new Contribution
            {
                Id = 6,
                DonorName = "Maria Toncheva",
                Amount = 140m,
                DonatedOn = new DateTime(2026, 3, 15),
                Note = "For new learning equipment.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Monthly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 1
            },
            new Contribution
            {
                Id = 7,
                DonorName = "Georgi Mihaylov",
                Amount = 60m,
                DonatedOn = new DateTime(2026, 3, 16),
                Note = "Keep going.",
                PaymentMethod = ContributionPaymentMethod.BankTransfer,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.PendingBankTransfer,
                FundingCampaignId = 1
            },
            new Contribution
            {
                Id = 8,
                DonorName = "Ralitsa Daneva",
                Amount = 210m,
                DonatedOn = new DateTime(2026, 3, 16),
                Note = "Community care matters.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 2
            },
            new Contribution
            {
                Id = 9,
                DonorName = "Nikolay Velikov",
                Amount = 95m,
                DonatedOn = new DateTime(2026, 3, 17),
                Note = "For additional screening kits.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Monthly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 2
            },
            new Contribution
            {
                Id = 10,
                DonorName = "Violeta Hristova",
                Amount = 180m,
                DonatedOn = new DateTime(2026, 3, 17),
                Note = "Proud of this work.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Weekly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 2
            },
            new Contribution
            {
                Id = 11,
                DonorName = "Anton Marinov",
                Amount = 70m,
                DonatedOn = new DateTime(2026, 3, 18),
                Note = "Art access should be public.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 3
            },
            new Contribution
            {
                Id = 12,
                DonorName = "Silvia Stoyanova",
                Amount = 130m,
                DonatedOn = new DateTime(2026, 3, 18),
                Note = "For the materials budget.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Monthly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 3
            },
            new Contribution
            {
                Id = 13,
                DonorName = "Kristian Popov",
                Amount = 55m,
                DonatedOn = new DateTime(2026, 3, 19),
                Note = "Looking forward to the studio week.",
                PaymentMethod = ContributionPaymentMethod.BankTransfer,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.PendingBankTransfer,
                FundingCampaignId = 3
            },
            new Contribution
            {
                Id = 14,
                DonorName = "Monika Petkova",
                Amount = 45m,
                DonatedOn = new DateTime(2026, 3, 19),
                Note = "Happy to support this cause.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 1
            },
            new Contribution
            {
                Id = 15,
                DonorName = "Stefan Iliev",
                Amount = 160m,
                DonatedOn = new DateTime(2026, 3, 20),
                Note = "From our family to yours.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.OneTime,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 2
            },
            new Contribution
            {
                Id = 16,
                DonorName = "Desislava Vasileva",
                Amount = 110m,
                DonatedOn = new DateTime(2026, 3, 20),
                Note = "Supporting local creativity.",
                PaymentMethod = ContributionPaymentMethod.Card,
                Frequency = ContributionFrequency.Weekly,
                Status = ContributionStatus.Successful,
                FundingCampaignId = 3
            });
    }
}
