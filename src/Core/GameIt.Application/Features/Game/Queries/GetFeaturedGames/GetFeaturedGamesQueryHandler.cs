using AutoMapper;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetFeaturedGames;

public class GetFeaturedGamesQueryHandler : IRequestHandler<GetFeaturedGamesQuery, List<GamesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFeaturedGamesQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GamesListDto>> Handle(GetFeaturedGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.Games.GetFeaturedGamesAsync(request.Limit, cancellationToken);
        if (games == null || !games.Any())
            return new List<GamesListDto>();

        return _mapper.Map<List<GamesListDto>>(games);
    }
}
