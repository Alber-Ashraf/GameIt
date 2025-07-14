using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Discount.Queries.GetActiveDiscounts;

public class GetActiveDiscountsQueryHandler : IRequestHandler<GetActiveDiscountsQuery, List<ActiveDiscountDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetActiveDiscountsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<List<ActiveDiscountDto>> Handle(
        GetActiveDiscountsQuery request,
        CancellationToken token)
    {
        var discounts = await _unitOfWork.Discounts
            .GetActiveDiscountsAsync(token);

        if (!discounts.Any())
            return new List<ActiveDiscountDto>();

        // Calculate prices
        var dtos = _mapper.Map<List<ActiveDiscountDto>>(discounts);
        dtos.ForEach(dto =>
        {
            dto.DiscountedPrice = dto.OriginalPrice * (100 - dto.Percentage / 100);
        });

        return dtos;
    }
}