using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");

        // Primary Key
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("PublisherName")
            .HasComment("Official publisher/developer name");

        builder.Property(p => p.LogoUrl)
            .IsRequired(false)
            .HasMaxLength(500)
            .HasColumnName("PublisherLogoUrl")
            .HasComment("URL to publisher's logo image");

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
        builder.HasMany(p => p.Games)
            .WithOne(g => g.Publisher)
            .HasForeignKey(g => g.PublisherId)
            .IsRequired(false)  // Games can exist without publisher
            .OnDelete(DeleteBehavior.SetNull)  // Preserve games if publisher is deleted
            .HasConstraintName("FK_Games_Publishers");
    }
}
