namespace GameIt.Application.Features.Library.Queries.GetUserLibrary;

public class LibraryListDto
{
    public Guid Id { get; set; }
    public Guid GameId => Id;
    public string Name { get; set; } = string.Empty;
    public string GameName => Name;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string GameImageUrl => ImageUrl;
    public long FileSizeInMB { get; set; }
    public string DownloadLink { get; set; } = string.Empty;
    public string? AdditionalDownloadLink { get; set; }
    public string SystemRequirements { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public DateTime PurchasedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string FileSizeFormatted => $"{FileSizeInMB:N0} MB";
}
