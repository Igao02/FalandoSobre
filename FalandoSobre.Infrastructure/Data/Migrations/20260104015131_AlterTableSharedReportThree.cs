using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FalandoSobre.Web.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableSharedReportThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "SharedReports",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SharedReports_ApplicationUserId",
                table: "SharedReports",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedReports_AspNetUsers_ApplicationUserId",
                table: "SharedReports",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedReports_AspNetUsers_ApplicationUserId",
                table: "SharedReports");

            migrationBuilder.DropIndex(
                name: "IX_SharedReports_ApplicationUserId",
                table: "SharedReports");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "SharedReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
