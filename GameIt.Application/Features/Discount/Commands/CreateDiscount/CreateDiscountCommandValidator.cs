using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Discount.Commands.CreateDiscount;

public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateDiscountCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("Game ID is required.")
            .MustAsync(GameExists).WithMessage("Game does not exist.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(1, 100).WithMessage("Discount must be 1-100%.");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past.")
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date.");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.UtcNow.Date)
            .WithMessage("End date must be in the future.");
    }
    private async Task<bool> GameExists(Guid gameId, CancellationToken token)
    {
        return await _unitOfWork.Games.ExistsAsync(gameId, token);
    }
}
