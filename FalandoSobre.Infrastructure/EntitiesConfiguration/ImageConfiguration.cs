using FalandoSobre.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FalandoSobre.Infrastructure.EntitiesConfiguration
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ImageUrl)
                .HasMaxLength(350)
                .IsRequired();

            builder.Property(i => i.ImageDate)
                .IsRequired();
        }
    }
}
