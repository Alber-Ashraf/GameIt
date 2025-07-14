using MediatR;

namespace GameIt.Application.Features.Discount.Queries.GetActiveDiscounts;

public class GetActiveDiscountsQuery : IRequest<List<ActiveDiscountDto>> { }
