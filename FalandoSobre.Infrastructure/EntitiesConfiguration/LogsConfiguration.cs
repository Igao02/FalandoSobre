using FalandoSobre.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FalandoSobre.Infrastructure.EntitiesConfiguration;

public class LogsConfiguration
{
    public void Configure(EntityTypeBuilder<Logs> builder)
    {
        builder
            .HasKey(l => l.Id);

        builder
            .Property(l => l.Id)
            .IsRequired();

        builder
            .Property(l => l.UserName)
            .IsRequired(false);

        builder
            .Property(l => l.Action)
            .IsRequired(false);

        builder
            .Property(l => l.EntityType)
            .IsRequired(false);

        builder
            .Property(l => l.ApplicationUserId)
            .IsRequired();

    }
}
