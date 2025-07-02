using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameLists
{
    public class GetAllGameListsQueryHandler : IRequestHandler<GetAllGameListsQuery, List<GameListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllGameListsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GameListDto>> Handle(GetAllGameListsQuery request, CancellationToken cancellationToken)
        {
            // Query the database for all games Lists
            var games = await _unitOfWork.Games.GetAllAsync();

            // Convert the game entities to GameDetailsDto using AutoMapper
            var data = _mapper.Map<List<GameListDto>>(games);

            // Return the list of GameDetailsDto
            return data;
        }
    }
}
