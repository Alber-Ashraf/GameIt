using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class ReviewMapping : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(x => x.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5");

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // Composite Index
            builder.HasIndex(r => new { r.UserId, r.GameId })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_UserGame");
        }
    }
}
