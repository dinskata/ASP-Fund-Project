using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignOwnershipAndApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "FundingCampaigns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "FundingCampaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "FundingCampaigns",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "af6f1f0d-5482-45c4-84dc-8d4c7ec9d001", null, "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "8cf2c7fc-3277-4bff-bc66-14f05fc4a001", 0, "8cf2c7fc-3277-4bff-bc66-14f05fc4a001", "admin@communityfundhub.local", true, "Platform Administrator", false, null, "ADMIN@COMMUNITYFUNDHUB.LOCAL", "ADMIN@COMMUNITYFUNDHUB.LOCAL", "AQAAAAIAAYagAAAAEFxuRvZxQm/vyVMytjxyb8RkvTTy6FwpftnnlpIAqG0SupBVNaciShn7W9j4//M+jA==", null, false, "8cf2c7fc-3277-4bff-bc66-14f05fc4a001", false, "admin@communityfundhub.local" });

            migrationBuilder.UpdateData(
                table: "FundingCampaigns",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "IsApproved", "OwnerId" },
                values: new object[] { new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "8cf2c7fc-3277-4bff-bc66-14f05fc4a001" });

            migrationBuilder.UpdateData(
                table: "FundingCampaigns",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedOn", "IsApproved", "OwnerId" },
                values: new object[] { new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "8cf2c7fc-3277-4bff-bc66-14f05fc4a001" });

            migrationBuilder.UpdateData(
                table: "FundingCampaigns",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedOn", "IsApproved", "OwnerId" },
                values: new object[] { new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "8cf2c7fc-3277-4bff-bc66-14f05fc4a001" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "af6f1f0d-5482-45c4-84dc-8d4c7ec9d001", "8cf2c7fc-3277-4bff-bc66-14f05fc4a001" });

            migrationBuilder.CreateIndex(
                name: "IX_FundingCampaigns_OwnerId",
                table: "FundingCampaigns",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundingCampaigns_AspNetUsers_OwnerId",
                table: "FundingCampaigns",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundingCampaigns_AspNetUsers_OwnerId",
                table: "FundingCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_FundingCampaigns_OwnerId",
                table: "FundingCampaigns");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "af6f1f0d-5482-45c4-84dc-8d4c7ec9d001", "8cf2c7fc-3277-4bff-bc66-14f05fc4a001" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "af6f1f0d-5482-45c4-84dc-8d4c7ec9d001");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8cf2c7fc-3277-4bff-bc66-14f05fc4a001");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "FundingCampaigns");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "FundingCampaigns");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "FundingCampaigns");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");
        }
    }
}
