using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Features.Game.Queries.GetGameDetails;

namespace GameIt.Application.Features.Category.Queries.GetCategoryDetails;

public class CategoryDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<GamesListDto> Games { get; set; } = new List<GamesListDto>();
}
