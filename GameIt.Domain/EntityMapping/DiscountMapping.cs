using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class DiscountMapping : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discounts");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Percentage)
                .IsRequired();

            builder.HasCheckConstraint("CK_Discounts_Percentage", "[Percentage] BETWEEN 0 AND 100");

            builder.Property(d => d.StartDate)
                .IsRequired();

            builder.Property(d => d.EndDate)
                .IsRequired();

            builder.Property(d => d.IsActive)
                .HasDefaultValue(true);

            builder.Property(d => d.Description)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(d => d.Game)
            .WithOne(g => g.Discount)
            .HasForeignKey<Discount>(d => d.GameId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
