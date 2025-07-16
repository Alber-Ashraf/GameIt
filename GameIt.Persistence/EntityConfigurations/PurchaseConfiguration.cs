using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.PurchaseDate)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.AmountPaid)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.OriginalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .HasColumnType("char(3)")
            .HasDefaultValue("USD")
            .IsFixedLength();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Timestamps 
        builder.Property(g => g.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(p => p.User)
            .WithMany(u => u.Purchases)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Game)
            .WithMany(g => g.Purchases)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        // Query Filter
        builder.HasQueryFilter(p => !p.IsRefunded);
    }
}
