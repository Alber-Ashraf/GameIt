using AutoMapper;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetGamesByCategory;

public class GetGamesByCategoryQueryHandler : IRequestHandler<GetGamesByCategoryQuery, List<GamesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGamesByCategoryQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GamesListDto>> Handle(GetGamesByCategoryQuery request, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.Games.GetGamesByCategoryAsync(request.CategoryId, request.Limit, cancellationToken);
        if (games == null || !games.Any())
            return new List<GamesListDto>();

        return _mapper.Map<List<GamesListDto>>(games);
    }
}
