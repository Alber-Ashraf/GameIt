using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Purchase.Commands.CreatePurchase;

public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("Game ID is required.")
            .MustAsync(GameExists).WithMessage("Game does not exist.");

        RuleFor(x => x.AmountPaid)
            .GreaterThanOrEqualTo(0).WithMessage("Amount paid must be greater than or equal to zero.");
    }

    private async Task<bool> GameExists(Guid gameId, CancellationToken token)
    {
        return await _unitOfWork.Games.ExistsAsync(gameId, token);
    }
}
