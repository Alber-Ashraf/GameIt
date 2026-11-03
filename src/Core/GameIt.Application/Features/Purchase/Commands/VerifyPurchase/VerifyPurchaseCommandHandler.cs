using System.Security.Claims;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Stripe.Checkout;

namespace GameIt.Application.Features.Purchase.Commands.VerifyPurchase;

public class VerifyPurchaseCommandHandler : IRequestHandler<VerifyPurchaseCommand, ConfirmPaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStripeService _stripeService;
    private readonly IHttpContextAccessor _contextAccessor;

    public VerifyPurchaseCommandHandler(
        IUnitOfWork unitOfWork,
        IStripeService stripeService,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _stripeService = stripeService;
        _contextAccessor = contextAccessor;
    }

    public async Task<ConfirmPaymentResult> Handle(VerifyPurchaseCommand request, CancellationToken token)
    {
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User must be authenticated.");
        }

        var purchase = await _unitOfWork.Purchases.GetByIdAsync(request.PurchaseId);
        if (purchase == null)
        {
            throw new NotFoundException(nameof(Domain.Purchase), request.PurchaseId);
        }

        if (purchase.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to verify this purchase.");
        }

        var game = await _unitOfWork.Games.GetByIdAsync(purchase.GameId);
        string gameName = game?.Name ?? "Game";

        if (purchase.Status == PaymentStatus.Succeeded)
        {
            // Ensure library entry exists if somehow missing
            var existingLib = await _unitOfWork.Libraries.GetLibraryForUserAsync(userId, token);
            if (!existingLib.Any(l => l.GameId == purchase.GameId))
            {
                await _unitOfWork.Libraries.CreateAsync(new Domain.Library
                {
                    UserId = userId,
                    GameId = purchase.GameId,
                    PurchasedAt = DateTime.UtcNow
                });
                await _unitOfWork.SaveChangesAsync(token);
            }

            return new ConfirmPaymentResult
            {
                IsSuccess = true,
                PurchaseId = purchase.Id,
                GameId = purchase.GameId,
                GameName = gameName,
                Status = PaymentStatus.Succeeded,
                Message = "Purchase has already been confirmed and added to your library."
            };
        }

        // Look up Stripe session
        Session? session = null;
        if (!string.IsNullOrWhiteSpace(purchase.StripePaymentIntentId) &&
            purchase.StripePaymentIntentId.StartsWith("cs_", StringComparison.OrdinalIgnoreCase))
        {
            session = await _stripeService.GetCheckoutSessionAsync(purchase.StripePaymentIntentId, token);
        }

        if (session == null)
        {
            session = await _stripeService.FindCheckoutSessionByPurchaseIdAsync(purchase.Id, token);
        }

        if (session == null)
        {
            return new ConfirmPaymentResult
            {
                IsSuccess = false,
                PurchaseId = purchase.Id,
                GameId = purchase.GameId,
                GameName = gameName,
                Status = purchase.Status,
                Message = "No matching checkout session found on Stripe."
            };
        }

        if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(session.Status, "complete", StringComparison.OrdinalIgnoreCase))
        {
            return new ConfirmPaymentResult
            {
                IsSuccess = false,
                PurchaseId = purchase.Id,
                GameId = purchase.GameId,
                GameName = gameName,
                Status = PaymentStatus.Pending,
                Message = $"Stripe reports payment is {session.PaymentStatus}."
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
            Message = "Payment verified successfully. Game added to your library."
        };
    }
}
