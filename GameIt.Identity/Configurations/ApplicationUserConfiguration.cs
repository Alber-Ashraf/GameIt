using GameIt.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        // Properties Configuration
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("LastName");

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(300)
            .HasColumnName("ProfilePictureUrl");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()")
            .HasColumnName("CreatedAt");

        builder.Property(u => u.LastLoginDate)
            .HasColumnName("LastLoginDate");

        // Indexes Configuration
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
