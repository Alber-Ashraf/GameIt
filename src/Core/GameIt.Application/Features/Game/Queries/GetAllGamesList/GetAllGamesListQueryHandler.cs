using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameLists;

public class GetAllGamesListQueryHandler : IRequestHandler<GetAllGamesListQuery, List<GamesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllGamesListQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GamesListDto>> Handle(GetAllGamesListQuery request, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.Games.GetAllWithCategoryAsync();
        if (games == null || !games.Any())
            return new List<GamesListDto>();

        return _mapper.Map<List<GamesListDto>>(games);
    }
}
