using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping
{
    public class PurchaseMapping : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases", schema: "GameIt");

            // Primary Key Configuration
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasDefaultValueSql("NEWID()")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(p => p.PurchaseDate)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("UTC timestamp of purchase");

            builder.Property(p => p.AmountPaid)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasComment("Final paid amount after discounts");

            builder.Property(p => p.OriginalPrice)
                .HasColumnType("decimal(18,2)")
                .HasComment("List price at time of purchase");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsFixedLength()
                .HasComment("ISO 4217 currency code");

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Payment gateway reference");

            builder.Property(p => p.PaymentStatus)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Completed")
                .HasConversion<string>();

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Purchases_Users");

            builder.HasOne(p => p.Game)
                .WithMany(g => g.Purchases)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Purchases_Games");

            // Query Filter
            builder.HasQueryFilter(p => !p.IsRefunded);
        }
    }
}
