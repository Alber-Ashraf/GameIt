using MediatR;

namespace GameIt.Application.Features.Game.Commands.UpdateGame;

public class UpdateGameCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public bool? IsFree { get; set; } = false;
    public bool? IsFeatured { get; set; } = false;
    public long FileSizeInMB { get; set; }
    public string? DownloadLink { get; set; }
    public string? AdditionalDownloadLink { get; set; }
    public string? SystemRequirements { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public Guid CategoryId { get; set; }
    public string? PublisherId { get; set; }
}
