using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GameIt.Persistence.IntegrationTests;

public class GameItDatabaseSeederTests
{
    private readonly DbContextOptions<GameItDbContext> _options;

    public GameItDatabaseSeederTests()
    {
        _options = new DbContextOptionsBuilder<GameItDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task SeedAsync_WhenDatabaseIsEmpty_ShouldSeedCategoriesAnd50Games()
    {
        // Arrange
        await using var context = new GameItDbContext(_options);

        // Act
        await GameItDatabaseSeeder.SeedAsync(context);

        // Assert
        var categories = await context.Categories.ToListAsync();
        categories.Count.ShouldBeGreaterThanOrEqualTo(10);

        var games = await context.Games.Include(g => g.Category).ToListAsync();
        games.Count.ShouldBe(50);

        // Verify download links are empty/null as requested
        games.ShouldAllBe(g => string.IsNullOrEmpty(g.DownloadLink));

        // Verify file sizes are stored in megabytes (not bytes)
        games.ShouldAllBe(g => g.FileSizeInMB.HasValue && g.FileSizeInMB.Value > 0 && g.FileSizeInMB.Value < 1_000_000);

        // Verify all games have valid categories assigned
        games.ShouldAllBe(g => g.CategoryId != Guid.Empty && g.Category != null);

        // Verify free games have IsFree = true
        var freeGames = games.Where(g => g.IsFree).ToList();
        freeGames.ShouldNotBeEmpty();
        freeGames.ShouldAllBe(g => g.Price == 0m);

        // Verify featured games exist
        var featuredGames = games.Where(g => g.IsFeatured).ToList();
        featuredGames.ShouldNotBeEmpty();

        // Verify discounts were seeded
        var discounts = await context.Discounts.ToListAsync();
        discounts.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SeedAsync_WhenCalledMultipleTimes_ShouldBeIdempotent()
    {
        // Arrange
        await using var context = new GameItDbContext(_options);

        // Act
        await GameItDatabaseSeeder.SeedAsync(context);
        await GameItDatabaseSeeder.SeedAsync(context);

        // Assert
        var gameCount = await context.Games.CountAsync();
        gameCount.ShouldBe(50);
    }
}
