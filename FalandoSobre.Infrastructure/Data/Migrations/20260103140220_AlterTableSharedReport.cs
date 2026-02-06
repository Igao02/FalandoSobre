using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FalandoSobre.Web.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableSharedReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SharedReports_ReportId",
                table: "SharedReports",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedReports_Reports_ReportId",
                table: "SharedReports",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedReports_Reports_ReportId",
                table: "SharedReports");

            migrationBuilder.DropIndex(
                name: "IX_SharedReports_ReportId",
                table: "SharedReports");
        }
    }
}
