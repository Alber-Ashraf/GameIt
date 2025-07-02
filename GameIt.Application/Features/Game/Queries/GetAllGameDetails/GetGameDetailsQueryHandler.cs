using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameDetails
{
    public class GetGameDetailsQueryHandler : IRequestHandler<GetGameDetailsQuery, GameDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetGameDetailsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GameDetailsDto> Handle(GetGameDetailsQuery request, CancellationToken cancellationToken)
        {
            // Query the database for all games Details
            var game = await _unitOfWork.Games.GetByIdWithDetailsAsync(request.Id);

            // Convert the game entities to GameDetailsDto using AutoMapper
            var data = _mapper.Map<GameDetailsDto>(game);

            // Return the list of GameDetailsDto
            return data;
        }
    }
}
