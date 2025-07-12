using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // Primary Key Configuration
        builder.HasKey(c => c.Id);

        // Properties Configuration
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("CategoryName")
            .HasComment("Official category name for grouping games");

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
        builder.HasMany(c => c.Games)
            .WithOne(g => g.Category)
            .HasForeignKey(g => g.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent accidental category deletion

        // Indexs
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Categories_Name");
    }
}
