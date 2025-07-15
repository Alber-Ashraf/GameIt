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
            .GreaterThanOrEqualTo(0).WithMessage("Amount paid must be greater than zero.");

        RuleFor(x => x.OriginalPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Original price must be greater than or equal to zero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter code (e.g., USD).");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Payment method is invalid.");

        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("Transaction ID is required.")
            .Length(1, 100).WithMessage("Transaction ID must be between 1 and 100 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Length(1, 100).WithMessage("User ID must be between 1 and 100 characters.");

        RuleFor(x => x)
            .Must(x => x.AmountPaid <= x.OriginalPrice)
            .WithMessage("Amount paid cannot exceed the original price.");
    }
    private async Task<bool> GameExists(Guid gameId, CancellationToken token)
    {
        return await _unitOfWork.Games.ExistsAsync(gameId, token);
    }
}
