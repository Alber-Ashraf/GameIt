using AutoMapper;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetSimilarGames;

public class GetSimilarGamesQueryHandler : IRequestHandler<GetSimilarGamesQuery, List<GamesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSimilarGamesQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GamesListDto>> Handle(GetSimilarGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.Games.GetSimilarGamesAsync(request.GameId, request.Limit, cancellationToken);
        if (games == null || !games.Any())
            return new List<GamesListDto>();

        return _mapper.Map<List<GamesListDto>>(games);
    }
}
