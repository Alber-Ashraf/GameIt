using GameIt.Domain.Common;

namespace GameIt.Domain;

public class Discount : BaseEntity
{
    public Guid GameId { get; set; }
    public Game Game { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Description { get; set; }
}
