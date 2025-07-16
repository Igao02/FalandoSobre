using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FalandoSobre.Web.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Institutions_AspNetUsers_ApplicationUserId",
                table: "Institutions");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Institutions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerfilImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Institutions_AspNetUsers_ApplicationUserId",
                table: "Institutions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Institutions_AspNetUsers_ApplicationUserId",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "PerfilImageUrl",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Institutions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Institutions_AspNetUsers_ApplicationUserId",
                table: "Institutions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
