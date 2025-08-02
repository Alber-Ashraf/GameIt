using GameIt.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Persistence.EntityConfigurations;

public class LibraryConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("Libraries");
        // Primary Key
        builder.HasKey(l => l.Id);
        // Properties
        builder.Property(l => l.UserId)
            .IsRequired()
            .HasMaxLength(450);
        // Relationships
        builder.HasOne(l => l.Game)
            .WithMany(g => g.Libraries)
            .HasForeignKey(l => l.GameId);
    }
}
