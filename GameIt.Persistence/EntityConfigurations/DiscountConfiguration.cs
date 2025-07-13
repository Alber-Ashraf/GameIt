using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");

        // Primary Key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Percentage)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(d => d.StartDate)
            .HasColumnType("datetime2");

        builder.Property(d => d.EndDate)
            .HasColumnType("datetime2");

        builder.Property(d => d.IsActive)
            .HasDefaultValue(true);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        // Timestamps
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Constraints
        builder.HasCheckConstraint("CK_Discounts_Percentage", "[Percentage] BETWEEN 0 AND 100");
        builder.HasCheckConstraint("CK_Discounts_DateRange", "[StartDate] < [EndDate]");

        // Relationship
        builder.HasOne(d => d.Game)
            .WithOne(g => g.Discount)
            .HasForeignKey<Discount>(d => d.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
