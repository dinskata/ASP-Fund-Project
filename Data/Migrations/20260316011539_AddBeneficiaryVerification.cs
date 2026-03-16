using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiaryVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Beneficiaries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Beneficiaries");
        }
    }
}
