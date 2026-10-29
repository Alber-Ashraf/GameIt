using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using GameIt.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace GameIt.Persistence.IntegrationTests;

public class GameRepositoryTests
{
    private static GameItDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GameItDbContext>()
            .UseInMemoryDatabase(databaseName: $"GameItRepoTest_{Guid.NewGuid()}")
            .Options;
        return new GameItDbContext(options);
    }

    [Fact]
    public async Task GetSimilarGamesAsync_WhenBaseGameDoesNotExist_ReturnsEmptyListWithoutNullReference()
    {
        // Arrange
        await using var context = CreateContext();
        var repo = new GameRepository(context);

        // Act
        var result = await repo.GetSimilarGamesAsync(Guid.NewGuid(), 5);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllWithCategoryAsync_IncludesCategoryDiscountAndReviews()
    {
        // Arrange
        await using var context = CreateContext();
        var categoryId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = "Action RPG"
        };

        var game = new Game
        {
            Id = gameId,
            Name = "Elden Chronicles",
            Description = "Open world adventure",
            Price = 59.99m,
            IsFree = false,
            IsFeatured = true,
            CategoryId = categoryId,
            ReleaseDate = DateTime.UtcNow.AddMonths(-1)
        };

        var discount = new Discount
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            Percentage = 20m,
            Description = "Autumn Sale",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        var review = new Review
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            UserId = "player-1",
            Rating = 5,
            Comment = "Incredible world design!"
        };

        await context.Categories.AddAsync(category);
        await context.Games.AddAsync(game);
        await context.Discounts.AddAsync(discount);
        await context.Reviews.AddAsync(review);
        await context.SaveChangesAsync();

        var repo = new GameRepository(context);

        // Act
        var games = await repo.GetAllWithCategoryAsync();

        // Assert
        games.Count.ShouldBe(1);
        var loadedGame = games[0];
        loadedGame.Category.ShouldNotBeNull();
        loadedGame.Category.Name.ShouldBe("Action RPG");
        loadedGame.Discount.ShouldNotBeNull();
        loadedGame.Discount.Percentage.ShouldBe(20m);
        loadedGame.Reviews.ShouldNotBeNull();
        loadedGame.Reviews.Count.ShouldBe(1);
        loadedGame.Reviews.Average(r => r.Rating).ShouldBe(5.0);
    }

    [Fact]
    public async Task GetSimilarGamesAsync_WhenGamesInSameCategoryExist_ReturnsOtherGamesOrderedByRating()
    {
        // Arrange
        await using var context = CreateContext();
        var categoryId = Guid.NewGuid();
        var baseGameId = Guid.NewGuid();
        var similarGame1Id = Guid.NewGuid();
        var similarGame2Id = Guid.NewGuid();

        await context.Categories.AddAsync(new Category { Id = categoryId, Name = "Sci-Fi" });
        await context.Games.AddRangeAsync(
            new Game { Id = baseGameId, Name = "Base Sci-Fi", Description = "Base", Price = 30m, CategoryId = categoryId },
            new Game { Id = similarGame1Id, Name = "Similar Low Rated", Description = "Sim 1", Price = 25m, CategoryId = categoryId },
            new Game { Id = similarGame2Id, Name = "Similar High Rated", Description = "Sim 2", Price = 35m, CategoryId = categoryId }
        );
        await context.Reviews.AddRangeAsync(
            new Review { Id = Guid.NewGuid(), GameId = similarGame1Id, UserId = "u1", Rating = 3, Comment = "Good" },
            new Review { Id = Guid.NewGuid(), GameId = similarGame2Id, UserId = "u2", Rating = 5, Comment = "Amazing" }
        );
        await context.SaveChangesAsync();

        var repo = new GameRepository(context);

        // Act
        var similar = await repo.GetSimilarGamesAsync(baseGameId, 5);

        // Assert
        similar.Count.ShouldBe(2);
        similar[0].Id.ShouldBe(similarGame2Id);
        similar[1].Id.ShouldBe(similarGame1Id);
    }
}
