using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class DiscountMapping : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discounts");

            // Primary Key Configuration
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id)
                .HasDefaultValueSql("NEWID()");

            // Properties Configuration
            builder.Property(d => d.Percentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)")  // Stores values like 15.50%
                .HasComment("Discount percentage (0-100)");

            builder.HasCheckConstraint("CK_Discounts_Percentage", "[Percentage] BETWEEN 0 AND 100");

            // Date Configuration
            builder.Property(d => d.StartDate)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasComment("Start date of the discount period");

            builder.Property(d => d.EndDate)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasComment("End date of the discount period");

            builder.HasCheckConstraint("CK_Discounts_DateRange", "[StartDate] < [EndDate] AND [EndDate] > GETDATE()");

            // Status Flags
            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("Whether discount is currently active");

            // Metadata
            builder.Property(d => d.Description)
                .HasMaxLength(500)
                .IsRequired(false)
                .HasComment("Optional description of the discount");

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
            builder.HasOne(d => d.Game)
                .WithOne(g => g.Discount)
                .HasForeignKey<Discount>(d => d.GameId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
