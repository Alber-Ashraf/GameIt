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
        _stripeService  = stripeService;
        _contextAccessor = contextAccessor;
    }

    public async Task<PurchaseResult> Handle(CreatePurchaseCommand request, CancellationToken token)
    {
        // Validate
        var validator = new CreatePurchaseCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Purchase", validationResult);

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Get game info
        var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(request.GameId, token);
        if (game == null)
            throw new NotFoundException(nameof(Game), request.GameId);

        // Create purchase (initial)
        var purchase = new Domain.Purchase
        {
            GameId = game.Id,
            UserId = userId,
            OriginalPrice = game.Price,
            AmountPaid = request.AmountPaid,
            PurchaseDate = DateTime.UtcNow,
            Status = PaymentStatus.Pending
        };

        PurchaseResult result;

        if (request.AmountPaid > 0)
        {
            // Call Stripe checkout
            var checkoutSession = await _stripeService.CreateCheckoutSessionAsync(
                request.AmountPaid,
                userId,
                request.GameId,
                token);

            purchase.StripePaymentIntentId = checkoutSession.StripePaymentIntentId;
            // purchase.StripeSessionId = checkoutSession.SessionId; ← لو محتاج

            result = new PurchaseResult
            {
                Status = PaymentStatus.Pending,
                StripeCheckoutUrl = checkoutSession.CheckoutUrl // Important!
            };
        }
        else
        {
            purchase.Status = PaymentStatus.Succeeded;

            result = new PurchaseResult
            {
                Status = PaymentStatus.Succeeded,
                StripeCheckoutUrl = null
            };
        }

        await _unitOfWork.Purchases.CreateAsync(purchase);
        await _unitOfWork.SaveChangesAsync();

        return result;
    }

}

