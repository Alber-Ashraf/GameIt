using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("CategoryName");

        // Timestamps
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasMany(c => c.Games)
            .WithOne(g => g.Category)
            .HasForeignKey(g => g.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Categories_Name");
    }
}
