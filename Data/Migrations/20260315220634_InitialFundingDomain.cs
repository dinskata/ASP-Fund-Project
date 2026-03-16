using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialFundingDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FocusArea = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Story = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FundingCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    OrganizerName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    GoalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    BeneficiaryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundingCampaigns_Beneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Beneficiaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonorName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    FundingCampaignId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contributions_FundingCampaigns_FundingCampaignId",
                        column: x => x.FundingCampaignId,
                        principalTable: "FundingCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Beneficiaries",
                columns: new[] { "Id", "City", "FocusArea", "FullName", "Story" },
                values: new object[,]
                {
                    { 1, "Sofia", "After-school education", "Bright Path Youth Center", "Bright Path supports teenagers from underserved neighborhoods with tutoring, mentoring, and digital literacy workshops throughout the school year." },
                    { 2, "Plovdiv", "Community health", "St. Marina Health Circle", "St. Marina Health Circle organizes mobile screenings, medicine support, and care coordination for elderly residents who struggle to access regular treatment." },
                    { 3, "Varna", "Creative inclusion", "Riverbank Arts Lab", "Riverbank Arts Lab runs free arts residencies and community events that help young creators turn talent into practical opportunities." }
                });

            migrationBuilder.InsertData(
                table: "FundingCampaigns",
                columns: new[] { "Id", "BeneficiaryId", "Category", "CurrentAmount", "EndDate", "GoalAmount", "IsFeatured", "OrganizerName", "StartDate", "Summary", "Title" },
                values: new object[,]
                {
                    { 1, 1, 1, 8450m, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000m, true, "Mila Petrova", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "A focused campaign to equip Bright Path students with refurbished laptops and internet vouchers for project-based learning.", "Laptop Library for Future Coders" },
                    { 2, 2, 2, 11200m, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 18000m, true, "Dragan Iliev", new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Funding one season of neighborhood screenings, blood pressure kits, and medication tracking support for homebound seniors.", "Mobile Screenings for Seniors" },
                    { 3, 3, 5, 3900m, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9000m, false, "Elena Georgieva", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A public-facing arts week that gives emerging creators materials, exhibition space, and mentoring from local professionals.", "Open Studio Week for Young Artists" }
                });

            migrationBuilder.InsertData(
                table: "Contributions",
                columns: new[] { "Id", "Amount", "DonatedOn", "DonorName", "FundingCampaignId", "Note" },
                values: new object[,]
                {
                    { 1, 250m, new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ivan Kolev", 1, "Happy to support practical tech access." },
                    { 2, 500m, new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nadya Hristova", 2, "For the mobile health visits." },
                    { 3, 120m, new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petar Nikolov", 3, "Excited to visit the exhibition." },
                    { 4, 320m, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Teodora Yaneva", 1, "Education changes trajectories." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_FundingCampaignId",
                table: "Contributions",
                column: "FundingCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_FundingCampaigns_BeneficiaryId",
                table: "FundingCampaigns",
                column: "BeneficiaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "FundingCampaigns");

            migrationBuilder.DropTable(
                name: "Beneficiaries");
        }
    }
}
