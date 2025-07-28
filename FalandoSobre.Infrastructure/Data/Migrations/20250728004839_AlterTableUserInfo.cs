using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FalandoSobre.Web.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePhotoBytes",
                table: "UserInfos",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhotoBytes",
                table: "UserInfos");
        }
    }
}
