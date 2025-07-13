using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");

        // Composite Primary Key
        builder.HasKey(w => new { w.UserId, w.GameId });

        // Timestamps 
        builder.Property(g => g.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Game)
            .WithMany(g => g.Wishlists)
            .HasForeignKey(w => w.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(w => w.GameId);
    }
}
