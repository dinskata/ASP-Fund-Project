using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreContributionsAndFixBeneficiaryDisplay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Contributions",
                columns: new[] { "Id", "Amount", "DonatedOn", "DonorName", "Frequency", "FundingCampaignId", "Note", "PaymentMethod", "Status" },
                values: new object[,]
                {
                    { 5, 85m, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kalin Georgiev", 1, 1, "Wishing the students a strong spring term.", 2, 1 },
                    { 6, 140m, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maria Toncheva", 3, 1, "For new learning equipment.", 2, 1 },
                    { 7, 60m, new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Georgi Mihaylov", 1, 1, "Keep going.", 1, 3 },
                    { 8, 210m, new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ralitsa Daneva", 1, 2, "Community care matters.", 2, 1 },
                    { 9, 95m, new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nikolay Velikov", 3, 2, "For additional screening kits.", 2, 1 },
                    { 10, 180m, new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Violeta Hristova", 2, 2, "Proud of this work.", 2, 1 },
                    { 11, 70m, new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anton Marinov", 1, 3, "Art access should be public.", 2, 1 },
                    { 12, 130m, new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Silvia Stoyanova", 3, 3, "For the materials budget.", 2, 1 },
                    { 13, 55m, new DateTime(2026, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kristian Popov", 1, 3, "Looking forward to the studio week.", 1, 3 },
                    { 14, 45m, new DateTime(2026, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monika Petkova", 1, 1, "Happy to support this cause.", 2, 1 },
                    { 15, 160m, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stefan Iliev", 1, 2, "From our family to yours.", 2, 1 },
                    { 16, 110m, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Desislava Vasileva", 2, 3, "Supporting local creativity.", 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
