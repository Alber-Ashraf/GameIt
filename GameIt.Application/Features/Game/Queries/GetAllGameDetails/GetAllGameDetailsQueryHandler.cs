using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameDetails
{
    public class GetAllGameDetailsQueryHandler : IRequestHandler<GetAllGameDetailsQuery, List<GameDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllGameDetailsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<GameDetailsDto>> Handle(GetAllGameDetailsQuery request, CancellationToken cancellationToken)
        {
            // Query the database for all games Details
            var games = await _unitOfWork.Games.GetAllAsync();

            // Convert the game entities to GameDetailsDto using AutoMapper
            var data = _mapper.Map<List<GameDetailsDto>>(games);

            // Return the list of GameDetailsDto
            return data;
        }
    }
}
