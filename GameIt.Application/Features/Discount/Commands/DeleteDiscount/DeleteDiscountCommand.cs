using MediatR;

namespace GameIt.Application.Features.Discount.Commands.DeleteDiscount;

public class DeleteDiscountCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}