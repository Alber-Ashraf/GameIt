using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class WishlistMapping : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");

            // Primary Key
            builder.HasKey(w => new { w.UserId, w.GameId });

            // Timestamps
            builder.Property(w => w.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd()
                .HasComment("When the item was added to wishlist");

            builder.Property(w => w.UpdatedAt)
                .IsRequired(false)
                .ValueGeneratedOnUpdate()
                .HasComment("Last modification timestamp");

            // Relationships
            builder.HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Wishlists_Users");

            builder.HasOne(w => w.Game)
                .WithMany(g => g.Wishlists)
                .HasForeignKey(w => w.GameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Wishlists_Games");

            // Unique Index on UserId and GameId
            builder.HasIndex(w => new { w.UserId, w.GameId })
                .IsUnique()
                .HasDatabaseName("IX_Wishlists_User_Game");

            builder.HasIndex(w => w.GameId);
        }
    }
}
