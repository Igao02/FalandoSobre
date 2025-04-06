using FalandoSobre.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FalandoSobre.Infrastructure.EntitiesConfiguration;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.LikeDate)
            .IsRequired();
    }
}

