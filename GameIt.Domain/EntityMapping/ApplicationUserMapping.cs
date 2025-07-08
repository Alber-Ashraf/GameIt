using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class ApplicationUserMapping : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");

            // Properties Configuration
            builder.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("DisplayName");

            builder.Property(u => u.ProfilePictureUrl)
                .HasMaxLength(300)
                .HasColumnName("ProfilePictureUrl");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("CreatedAt");

            builder.Property(u => u.LastLoginDate)
                .HasColumnName("LastLoginDate");

            // Relationships Configuration
            builder.HasMany(u => u.Purchases)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Wishlists)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes Configuration
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
