using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class PurchaseMapping : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PurchaseDate)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Game)
                .WithMany(g => g.Purchases)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
