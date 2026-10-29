using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Purchase.Commands.CreatePurchase;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStripeService _stripeService;
    private readonly IHttpContextAccessor _contextAccessor;

    public CreatePurchaseCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IStripeService stripeService,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _stripeService = stripeService;
        _contextAccessor = contextAccessor;
    }

    public async Task<PurchaseResult> Handle(CreatePurchaseCommand request, CancellationToken token)
    {
        var validator = new CreatePurchaseCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Purchase", validationResult);

        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(request.GameId, token);
        if (game == null)
            throw new NotFoundException(nameof(Game), request.GameId);

        var existingLibrary = await _unitOfWork.Libraries.GetLibraryForUserAsync(userId, token);
        if (existingLibrary.Any(l => l.GameId == game.Id))
            throw new BadRequestException("You already own this game in your library.");

        decimal basePrice = game.IsFree ? 0m : (game.Price ?? 0m);
        decimal authoritativePrice = basePrice;

        if (!game.IsFree && game.Discount != null && game.Discount.IsActive &&
            game.Discount.StartDate <= DateTime.UtcNow && game.Discount.EndDate >= DateTime.UtcNow &&
            game.Discount.Percentage is > 0m and <= 100m)
        {
            authoritativePrice = Math.Round(basePrice * (1m - (game.Discount.Percentage / 100m)), 2);
        }

        var purchase = new Domain.Purchase
        {
            GameId = game.Id,
            UserId = userId,
            OriginalPrice = basePrice,
            AmountPaid = authoritativePrice,
            PurchaseDate = DateTime.UtcNow,
            Status = PaymentStatus.Pending
        };

        PurchaseResult result;

        if (authoritativePrice > 0m)
        {
            var checkoutSession = await _stripeService.CreateCheckoutSessionAsync(
                authoritativePrice,
                userId,
                request.GameId,
                purchase.Id,
                token);

            purchase.StripePaymentIntentId = checkoutSession.StripePaymentIntentId;

            result = new PurchaseResult
            {
                Id = purchase.Id,
                Status = PaymentStatus.Pending,
                StripeCheckoutUrl = checkoutSession.CheckoutUrl,
                PurchaseDate = purchase.PurchaseDate
            };
        }
        else
        {
            purchase.Status = PaymentStatus.Succeeded;

            var libraryEntry = new Domain.Library
            {
                UserId = userId,
                GameId = game.Id,
                PurchasedAt = DateTime.UtcNow
            };
            await _unitOfWork.Libraries.CreateAsync(libraryEntry);

            result = new PurchaseResult
            {
                Id = purchase.Id,
                Status = PaymentStatus.Succeeded,
                StripeCheckoutUrl = null,
                PurchaseDate = purchase.PurchaseDate
            };
        }

        await _unitOfWork.Purchases.CreateAsync(purchase);
        await _unitOfWork.SaveChangesAsync(token);

        return result;
    }
}
