using MediatR;

namespace GameIt.Application.Features.Discount.Commands.CreateDiscount;

public class CreateDiscountCommand : IRequest<Guid>
{
    public Guid GameId { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
    