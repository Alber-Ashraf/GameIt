using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Purchase : BaseEntity
    {
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public decimal AmountPaid { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Currency { get; set; }
        public string TransactionId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public bool IsRefunded { get; set; }

        // Relationships
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }
    }
    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        Crypto,
        GiftCard
    }
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}
