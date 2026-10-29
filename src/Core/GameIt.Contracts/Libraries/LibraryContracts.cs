namespace GameIt.Contracts.Libraries;

public class LibraryItemDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameImageUrl { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public string? DownloadLink { get; set; }
    public long FileSizeInBytes { get; set; }
    public string? SystemRequirements { get; set; }
}
