using FalandoSobre.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FalandoSobre.Infrastructure.EntitiesConfiguration;

public class SharedReportConfiguration : IEntityTypeConfiguration<SharedReport>
{
    public void Configure(EntityTypeBuilder<SharedReport> builder)
    {
        builder.HasKey(sr => sr.Id);

        builder
            .Property(sr => sr.ReportId)
            .IsRequired();

        builder
            .Property(sr => sr.Actived)
            .IsRequired();

        builder
            .Property(sr => sr.CreatedAt)
            .IsRequired();

        builder
            .Property(sr => sr.UserName)
            .IsRequired();
    }
}
