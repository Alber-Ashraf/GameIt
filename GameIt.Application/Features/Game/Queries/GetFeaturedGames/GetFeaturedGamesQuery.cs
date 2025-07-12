using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetFeaturedGames;

public record GetFeaturedGamesQuery(int Limit = 5) : IRequest<List<GamesListDto>>;
