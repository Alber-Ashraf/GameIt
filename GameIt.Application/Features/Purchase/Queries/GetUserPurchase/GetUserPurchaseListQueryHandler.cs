using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public class GetUserPurchaseListQueryHandler : IRequestHandler<GetUserPurchaseListQuery, List<PurchaseListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserPurchaseListQueryHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PurchaseListDto>> Handle(
        GetUserPurchaseListQuery request,
        CancellationToken cancellationToken)
    {
        // Fetch purchases with related data
        var purchases = await _unitOfWork.Purchases
            .GetUserPurchasesAsync(request.UserId, cancellationToken);
        
        if (!purchases.Any())
            return new List<PurchaseListDto>();

        return _mapper.Map<List<PurchaseListDto>>(purchases);
    }
}
