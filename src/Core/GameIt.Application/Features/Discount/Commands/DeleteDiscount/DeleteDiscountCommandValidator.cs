using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Discount.Commands.DeleteDiscount;

public class DeleteDiscountCommandValidator : AbstractValidator<DeleteDiscountCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscountCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Discount ID is required.")
            .MustAsync(DiscountExists).WithMessage("Discount does not exist.");
    }

    private async Task<bool> DiscountExists(Guid DiscountId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Discounts.ExistsAsync(DiscountId, cancellationToken);
    }
}