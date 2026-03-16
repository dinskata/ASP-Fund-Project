using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_Fund_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContributorUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContributorUserId",
                table: "Contributions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_ContributorUserId",
                table: "Contributions",
                column: "ContributorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_AspNetUsers_ContributorUserId",
                table: "Contributions",
                column: "ContributorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_AspNetUsers_ContributorUserId",
                table: "Contributions");

            migrationBuilder.DropIndex(
                name: "IX_Contributions_ContributorUserId",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "ContributorUserId",
                table: "Contributions");
        }
    }
}
