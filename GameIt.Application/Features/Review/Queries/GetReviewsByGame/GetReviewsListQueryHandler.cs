using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Review.Queries.GetReviewsByGame;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace ReviewIt.Application.Features.Review.Queries.GetReviewDetails;

public record GetReviewsByGameQuery(Guid GameId) : IRequest<List<ReviewListDto>>;

public class GetReviewsByGameQueryHandler
    : IRequestHandler<GetReviewsByGameQuery, List<ReviewListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReviewsByGameQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ReviewListDto>> Handle(
        GetReviewsByGameQuery request,
        CancellationToken token)
    {
        var reviews = await _unitOfWork.Reviews
            .GetReviewsByGameAsync(request.GameId, token);

        return _mapper.Map<List<ReviewListDto>>(reviews);
    }
}
