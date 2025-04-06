using FalandoSobre.Domain.Entities;
using FalandoSobre.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FalandoSobre.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Report> Reports { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Like> Likes { get; set; }

    public DbSet<Image> Images { get; set; }

    public DbSet<Institution> Institutions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(FalandoSobreAssemblyReference.Assembly);
    }
}
