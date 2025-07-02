using AutoMapper;
using GameIt.Application.Exeptions;
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
            var existingGame = await _unitOfWork.Games.GetByIdWithDetailsAsync(request.Id);

            // Validate if the game exists
            if (existingGame == null)
                throw new NotFoundException(nameof(Game), request.Id);

            // Convert the game entities to GameDetailsDto using AutoMapper
            var data = _mapper.Map<GameDetailsDto>(existingGame);

            // Return the list of GameDetailsDto
            return data;
        }
    }
}
