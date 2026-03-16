using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationFlowAndBankDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountName",
                table: "Beneficiaries",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Beneficiaries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bic",
                table: "Beneficiaries",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "Beneficiaries",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Beneficiaries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BankAccountName", "BankName", "Bic", "Iban" },
                values: new object[] { "Bright Path Youth Center", "UniCredit Bulbank", "UNCRBGSF", "BG80UNCR70001512345678" });

            migrationBuilder.UpdateData(
                table: "Beneficiaries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BankAccountName", "BankName", "Bic", "Iban" },
                values: new object[] { "St. Marina Health Circle", "DSK Bank", "STSABGSF", "BG48STSA93000012345678" });

            migrationBuilder.UpdateData(
                table: "Beneficiaries",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BankAccountName", "BankName", "Bic", "Iban" },
                values: new object[] { "Riverbank Arts Lab", "Postbank", "BPBIBGSF", "BG30BPBI79401234567890" });

            migrationBuilder.UpdateData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Frequency", "PaymentMethod", "Status" },
                values: new object[] { 1, 2, 1 });

            migrationBuilder.UpdateData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Frequency", "PaymentMethod", "Status" },
                values: new object[] { 1, 1, 3 });

            migrationBuilder.UpdateData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Frequency", "PaymentMethod", "Status" },
                values: new object[] { 3, 2, 1 });

            migrationBuilder.UpdateData(
                table: "Contributions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Frequency", "PaymentMethod", "Status" },
                values: new object[] { 2, 2, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "BankAccountName",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "Bic",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "Beneficiaries");
        }
    }
}
