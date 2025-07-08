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
                .HasColumnName("GameName")
                .HasComment("Official game title");

            builder.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(1000)
                .HasComment("Detailed game description");

            builder.Property(g => g.ImageUrl)
                .IsRequired()
                .HasMaxLength(300)
                .HasComment("URL to game cover image");

            builder.Property(g => g.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0)
                .HasComment("Base price of the game");

            builder.Property(g => g.IsFree)
                .HasDefaultValue(false)
                .HasComment("Indicates if the game is free to play");

            builder.Property(g => g.IsFeatured)
                .HasDefaultValue(false)
                .HasComment("Indicates if the game is featured on the platform");

            builder.Property(g => g.Size)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Game file size (e.g., '15GB')");

            builder.Property(g => g.SystemRequirements)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .HasComment("JSON formatted system requirements for the game");

            builder.Property(g => g.DownloadLink)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Direct download link for the game");

            builder.Property(g => g.ReleaseDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Release date of the game");

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
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
