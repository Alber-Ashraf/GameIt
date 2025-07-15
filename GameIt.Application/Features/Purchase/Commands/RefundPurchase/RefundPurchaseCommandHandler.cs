using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.RefundPurchase;

public class RefundPurchaseCommandHandler : IRequestHandler<RefundPurchaseCommand, RefundResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    //private readonly IStripeService _stripeService;
    private readonly IMapper _mapper;

    public RefundPurchaseCommandHandler(
        IUnitOfWork unitOfWork,
        //IStripeService stripeService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        //_stripeService = stripeService;
        _mapper = mapper;
    }

    public async Task<RefundResponse> Handle(
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

        // Validate refund eligibility
        if (purchase.PaymentStatus != PaymentStatus.Completed)
            throw new BadRequestException("Purchase not eligible for refund");
        if (purchase.IsRefunded)
            throw new BadRequestException("Purchase already refunded");
        /*
        // Process Stripe refund
        var refundResult = await _stripeService.ProcessRefundAsync(
            purchase.TransactionId,
            purchase.AmountPaid,
            purchase.Currency,
            request.Reason,
            token);
        
        if (!RefundResponse.Success)
            throw new BadRequestException("Refund processing failed");
        */

        // Map request to purchase entity with additional fields
        purchase = _mapper.Map<Domain.Purchase>(request, opt =>
        {
            opt.Items["RefundDate"] = DateTime.UtcNow;
        });

        // Update purchase record
        _unitOfWork.Purchases.Update(purchase);

        // Save changes
        await _unitOfWork.SaveChangesAsync(token);

        // Return result
        return _mapper.Map<RefundResponse>(purchase);
    }
}
