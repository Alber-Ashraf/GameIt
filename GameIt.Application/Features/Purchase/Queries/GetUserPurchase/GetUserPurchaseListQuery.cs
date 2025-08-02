using MediatR;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public record GetUserPurchaseListQuery() : IRequest<List<PurchaseListDto>> {}