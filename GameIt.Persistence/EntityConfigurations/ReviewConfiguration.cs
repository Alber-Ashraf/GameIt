using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            // Primary Key
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasDefaultValueSql("NEWID()")
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(r => r.Rating)
                .IsRequired()
                .HasComment("Rating value (1-5 stars)");

            builder.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5");

            builder.Property(r => r.Comment)
                .HasMaxLength(1000)
                .IsRequired(false)
                .HasComment("Optional review text content");
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
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviews_Users");

            builder.HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviews_Games");

            // Unique Index on UserId and GameId

            builder.HasIndex(r => new { r.UserId, r.GameId })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_User_Game");
        }
    }
}
