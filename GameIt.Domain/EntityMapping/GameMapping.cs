using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class GameMapping : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            // Primary Key
            builder.HasKey(g => g.Id);

            // Properties Configuration
            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("GameName");

            builder.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(g => g.ImageUrl)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(g => g.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(g => g.IsFree)
                .HasDefaultValue(false);

            builder.Property(g => g.IsFeatured)
                .HasDefaultValue(false);

            builder.Property(g => g.Size)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.DownloadLink)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(g => g.ReleaseDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Timestamps
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false)
                .ValueGeneratedOnUpdate();

            // Relationships
            builder.HasOne(g => g.Category)
                .WithMany(c => c.Games)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Discount)
                .WithOne(d => d.Game)
                .HasForeignKey<Discount>(d => d.GameId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.Publisher)
                .WithMany(p => p.Games)
                .HasForeignKey(g => g.PublisherId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
