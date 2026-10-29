using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        // Primary Key
        builder.HasKey(g => g.Id);

        // Properties
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.ImageUrl)
            .HasMaxLength(300);

        builder.Property(g => g.Price)
            .HasColumnType("decimal(10,2)");

        builder.Property(g => g.IsFree)
            .HasDefaultValue(false);

        builder.Property(g => g.IsFeatured)
            .HasDefaultValue(false);

        builder.Property(g => g.FileSizeInBytes);

        builder.Property(g => g.SystemRequirements)
            .HasColumnType("nvarchar(max)");

        builder.Property(g => g.DownloadLink)
            .HasMaxLength(500);

        builder.Property(g => g.ReleaseDate)
            .HasDefaultValueSql("GETDATE()");

        // Timestamps 
        builder.Property(g => g.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(g => g.Category)
            .WithMany(c => c.Games)
            .HasForeignKey(g => g.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Discount)
            .WithOne(d => d.Game)
            .HasForeignKey<Discount>(d => d.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
