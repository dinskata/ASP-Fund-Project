using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiaryKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Beneficiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Beneficiaries",
                keyColumn: "Id",
                keyValue: 1,
                column: "Kind",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Beneficiaries",
                keyColumn: "Id",
                keyValue: 2,
                column: "Kind",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Beneficiaries",
                keyColumn: "Id",
                keyValue: 3,
                column: "Kind",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Beneficiaries");
        }
    }
}
