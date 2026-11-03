using System.Security.Claims;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GameIt.Application.Features.Purchase.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStripeService _stripeService;
    private readonly IHttpContextAccessor _contextAccessor;

    public ConfirmPaymentCommandHandler(
        IUnitOfWork unitOfWork,
        IStripeService stripeService,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _stripeService = stripeService;
        _contextAccessor = contextAccessor;
    }

    public async Task<ConfirmPaymentResult> Handle(ConfirmPaymentCommand request, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new BadRequestException("Session ID is required.");
        }

        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User must be authenticated.");
        }

        var session = await _stripeService.GetCheckoutSessionAsync(request.SessionId, token);
        if (session == null)
        {
            return new ConfirmPaymentResult
            {
                IsSuccess = false,
                Message = "Stripe checkout session not found."
            };
        }

        if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(session.Status, "complete", StringComparison.OrdinalIgnoreCase))
        {
            return new ConfirmPaymentResult
            {
                IsSuccess = false,
                Message = $"Payment is still processing (status: {session.PaymentStatus}).",
                Status = PaymentStatus.Pending
            };
        }

        Guid purchaseId = Guid.Empty;
        if (session.Metadata != null && session.Metadata.TryGetValue("purchase_id", out var pidStr))
        {
            Guid.TryParse(pidStr, out purchaseId);
        }

        Domain.Purchase? purchase = null;
        if (purchaseId != Guid.Empty)
        {
            purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId);
        }

        if (purchase == null)
        {
            return new ConfirmPaymentResult
            {
                IsSuccess = false,
                Message = "Associated purchase record was not found."
            };
        }

        if (purchase.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to confirm this purchase.");
        }

        var game = await _unitOfWork.Games.GetByIdAsync(purchase.GameId);
        string gameName = game?.Name ?? "Game";

        if (purchase.Status == PaymentStatus.Succeeded)
        {
            return new ConfirmPaymentResult
            {
                IsSuccess = true,
                PurchaseId = purchase.Id,
                GameId = purchase.GameId,
                GameName = gameName,
                Status = PaymentStatus.Succeeded,
                Message = "Payment already confirmed."
            };
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                purchase.Status = PaymentStatus.Succeeded;
                purchase.PurchaseDate = DateTime.UtcNow;
                purchase.StripePaymentIntentId = session.PaymentIntentId ?? session.Id;
                _unitOfWork.Purchases.Update(purchase);

                var libraryItems = await _unitOfWork.Libraries.GetLibraryForUserAsync(userId, token);
                if (!libraryItems.Any(l => l.GameId == purchase.GameId))
                {
                    await _unitOfWork.Libraries.CreateAsync(new Domain.Library
                    {
                        UserId = userId,
                        GameId = purchase.GameId,
                        PurchasedAt = DateTime.UtcNow
                    });
                }

                await _unitOfWork.SaveChangesAsync(token);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        return new ConfirmPaymentResult
        {
            IsSuccess = true,
            PurchaseId = purchase.Id,
            GameId = purchase.GameId,
            GameName = gameName,
            Status = PaymentStatus.Succeeded,
            Message = "Payment confirmed successfully."
        };
    }
}
