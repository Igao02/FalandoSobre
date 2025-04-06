using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FalandoSobre.Infrastructure.EntitiesConfiguration;

public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder
            .HasKey(i => i.Id);

        builder
            .Property(i => i.Id)
            .IsRequired();

        builder
            .Property(i => i.CorporateName)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(i => i.Document)
            .HasMaxLength(14)
            .IsRequired();

        builder
             .Property(i => i.Cep)
             .HasMaxLength(10)
             .IsRequired();

        builder
            .Property(i => i.City)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(i => i.Street)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(i => i.Neighborhood)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(i => i.Uf)
            .HasMaxLength(2)
            .IsRequired();

        builder
            .Property(i => i.NumHome)
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(i => i.Complement)
            .HasMaxLength(150);
    }
}
