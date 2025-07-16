using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.RefundPurchase;

public class RefundPurchaseCommandHandler : IRequestHandler<RefundPurchaseCommand, RefundResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStripeService _stripeService;

    public RefundPurchaseCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IStripeService stripeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _stripeService = stripeService;
    }

    public async Task<RefundResult> Handle(
        RefundPurchaseCommand request,
        CancellationToken token)
    {
        // Validate the request
        var validator = new RefundPurchaseCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Refund Request", validationResult);

        // Get purchase record
        var purchase = await _unitOfWork.Purchases.GetByIdAsync(request.PurchaseId);
        if (purchase == null)
            throw new NotFoundException(nameof(Purchase), request.PurchaseId);


        // Process Stripe refund
        var refundResult = await _stripeService.RefundPurchaseAsync(
        purchase.StripePaymentIntentId,
        token);

        if (!refundResult.Success)
            throw new BadRequestException("Refund processing failed");

        // Update purchase entity
        purchase.IsRefunded = true;
        purchase.RefundDate = DateTime.UtcNow;
        purchase.RefundId = refundResult.RefundId;
        purchase.Status = PaymentStatus.Refunded;

        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<RefundResult>(refundResult);
    }
}
