using FalandoSobre.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FalandoSobre.Infrastructure.EntitiesConfiguration;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder
            .HasKey(u => u.Id);

        builder
            .Property(u => u.Id)
            .IsRequired();

        builder
            .Property(u => u.ProfilePhoto)
            .HasMaxLength(200)
            .IsRequired(false);

        builder
            .Property(u => u.Actived)
            .IsRequired(false);

        builder
            .Property(u => u.CreatedAt)
            .IsRequired(false);

        builder
            .Property(u => u.ApplicationUserId)
            .IsRequired();

        builder
            .Property(u => u.ProfilePhotoBytes)
            .IsRequired(false);
    }
}
