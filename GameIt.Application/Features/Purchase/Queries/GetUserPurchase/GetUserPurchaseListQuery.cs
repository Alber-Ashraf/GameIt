using MediatR;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public record GetUserPurchaseListQuery(string UserId) : IRequest<List<PurchaseListDto>> {}