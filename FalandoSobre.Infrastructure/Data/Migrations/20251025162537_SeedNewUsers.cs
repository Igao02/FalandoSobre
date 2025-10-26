using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FalandoSobre.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedNewUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var userId = Guid.NewGuid();
            migrationBuilder.Sql($@"
                INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName,Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, LockoutEnabled, TwoFactorEnabled, AccessFailedCount)
                VALUES 
                ('{userId}', 'majestade_supremaa', 'MAJESTADE_SUPREMAA', 'igorabiezer08@gmail.com', 'IGORABIEZER08@GMAIL.COM', 1, '@Teste123', 
                'TY66N7MOYHCDAVADN2WZFZKIMDHKFUMS', '607269a2-07df-4a9c-8bcb-62a4613b9ca4', 0, 1, 0, 0);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Users 
                WHERE Email = 'igorabiezer08@gmail.com';
            ");
        }
    }
}
