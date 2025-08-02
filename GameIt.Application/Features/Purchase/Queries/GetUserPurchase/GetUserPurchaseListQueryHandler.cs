using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public class GetUserPurchaseListQueryHandler : IRequestHandler<GetUserPurchaseListQuery, List<PurchaseListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetUserPurchaseListQueryHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor contextAccessor)

    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<PurchaseListDto>> Handle(
        GetUserPurchaseListQuery request,
        CancellationToken cancellationToken)
    {
        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Fetch purchases with related data
        var purchases = await _unitOfWork.Purchases
            .GetUserPurchasesAsync(userId, cancellationToken);
        
        if (!purchases.Any())
            return new List<PurchaseListDto>();

        return _mapper.Map<List<PurchaseListDto>>(purchases);
    }
}
