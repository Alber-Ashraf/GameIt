using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Discount.Commands.UpdateDiscount;

public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDiscountCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Discount ID is required.")
            .MustAsync(DiscountExists).WithMessage("Discount does not exist.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(1, 100).WithMessage("Discount must be 1-100%.");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past.")
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date.");
    }
    private async Task<bool> DiscountExists(Guid discountId, CancellationToken token)
    {
        return await _unitOfWork.Games.ExistsAsync(discountId, token);
    }
}