using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiaryManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerUserId",
                table: "Beneficiaries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_ManagerUserId",
                table: "Beneficiaries",
                column: "ManagerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_AspNetUsers_ManagerUserId",
                table: "Beneficiaries",
                column: "ManagerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beneficiaries_AspNetUsers_ManagerUserId",
                table: "Beneficiaries");

            migrationBuilder.DropIndex(
                name: "IX_Beneficiaries_ManagerUserId",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "ManagerUserId",
                table: "Beneficiaries");
        }
    }
}
