using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FalandoSobre.Web.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurationUserReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Reports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Institutions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ApplicationUserId",
                table: "Reports",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_ApplicationUserId",
                table: "Institutions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Institutions_AspNetUsers_ApplicationUserId",
                table: "Institutions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ApplicationUserId",
                table: "Reports",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Institutions_AspNetUsers_ApplicationUserId",
                table: "Institutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_ApplicationUserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ApplicationUserId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ApplicationUserId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Likes_ApplicationUserId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Institutions_ApplicationUserId",
                table: "Institutions");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Comments");
        }
    }
}
