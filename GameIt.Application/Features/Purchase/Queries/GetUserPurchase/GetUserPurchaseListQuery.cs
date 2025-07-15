using MediatR;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public class GetUserPurchaseListQuery : IRequest<List<PurchaseListDto>> {}