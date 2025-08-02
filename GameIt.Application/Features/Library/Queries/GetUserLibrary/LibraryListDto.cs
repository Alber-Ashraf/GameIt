namespace GameIt.Application.Features.Library.Queries.GetUserLibrary;

public class LibraryListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; }
    public string DownloadLink { get; set; } = string.Empty;
    public string SystemRequirements { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public DateTime PurchasedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string FileSizeFormatted =>
        FileSizeInBytes >= 1024 * 1024 * 1024
            ? $"{FileSizeInBytes / (1024 * 1024 * 1024.0):0.##} GB"
            : $"{FileSizeInBytes / (1024 * 1024.0):0.##} MB";
}
