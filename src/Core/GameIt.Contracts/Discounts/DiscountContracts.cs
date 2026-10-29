using System.ComponentModel.DataAnnotations;

namespace GameIt.Contracts.Discounts;

public class ActiveDiscountItemDto
{
    public Guid Id { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public Guid GameId { get; set; }
}

public class UpsertDiscountRequestDto
{
    public Guid Id { get; set; }

    [Range(0.01, 100, ErrorMessage = "Discount percentage must be between 0.01% and 100%.")]
    public decimal Percentage { get; set; } = 20m;

    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddMinutes(5);
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Select a game for this discount.")]
    public Guid GameId { get; set; }
}
