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
            builder.HasKey(w => w.Id);

            // Relationships

            builder.HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Game)
                .WithMany(g => g.Wishlists)
                .HasForeignKey(r => r.GameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
