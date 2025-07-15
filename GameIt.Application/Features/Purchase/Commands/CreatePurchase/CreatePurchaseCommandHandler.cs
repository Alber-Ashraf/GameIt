using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.CreatePurchase;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    //private readonly IPaymentService _paymentService;

    public CreatePurchaseCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork
        /*IPaymentService paymentService*/)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        /*_paymentService = paymentService*/;
    }

    public async Task<PurchaseResponse> Handle(
        CreatePurchaseCommand request,
        CancellationToken token)
    {
        // Validate
        var validator = new CreatePurchaseCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Purchase", validationResult);

        // Verify game exists
        if (!await _unitOfWork.Games.ExistsAsync(request.GameId, token))
            throw new NotFoundException(nameof(Game), request.GameId);
        /*
        // Process payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(
            request.AmountPaid,
            request.Currency,
            request.TransactionId,
            token);
        */
        // Map command to entity with additional fields
        var purchase = _mapper.Map<Domain.Purchase>(request, opt =>
        {
            //opt.Items["PaymentStatus"] = paymentResult.Status;
            opt.Items["PurchaseDate"] = DateTime.UtcNow;
        });

        await _unitOfWork.Purchases.CreateAsync(purchase);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<PurchaseResponse>(purchase);
    }
}
