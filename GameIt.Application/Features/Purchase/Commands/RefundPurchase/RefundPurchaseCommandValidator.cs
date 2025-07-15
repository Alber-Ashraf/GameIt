using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Purchase.Commands.RefundPurchase;

public class RefundPurchaseCommandValidator : AbstractValidator<RefundPurchaseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RefundPurchaseCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.PurchaseId)
            .NotEmpty()
            .MustAsync(PurchaseExists).WithMessage("Purchase not found");

        RuleFor(x => x.AdminUserId)
            .NotEmpty().WithMessage("Admin user ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Refund reason is required")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
    }

    private async Task<bool> PurchaseExists(Guid purchaseId, CancellationToken token)
    {
        return await _unitOfWork.Purchases.ExistsAsync(purchaseId, token);
    }
}