using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public class GetUserPurchaseListQueryHandler : IRequestHandler<GetUserPurchaseListQuery, List<PurchaseListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    //private readonly ICurrentUserService _currentUserService;

    public GetUserPurchaseListQueryHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork/*,
        ICurrentUserService currentUserService*/)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        //_currentUserService = currentUserService;
    }

    public async Task<List<PurchaseListDto>> Handle(
        GetUserPurchaseListQuery request,
        CancellationToken cancellationToken)
    {

        // Get current user ID
        //var userId = _currentUserService.UserId;

        // For demonstration purposes, using a hardcoded user ID
        var userId = "id";

        // Fetch purchases with related data
        var purchases = await _unitOfWork.Purchases
            .GetUserPurchasesAsync(userId, cancellationToken);
        
        if (!purchases.Any())
            return new List<PurchaseListDto>();

        return _mapper.Map<List<PurchaseListDto>>(purchases);
    }
}
