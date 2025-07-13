using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        // Primary Key (assuming BaseEntity handles Id)
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        // Timestamps 
        builder.Property(g => g.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);

        // Constraints
        builder.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5");

        // Relationships
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Game)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(r => new { r.UserId, r.GameId })
            .IsUnique();
    }
}
