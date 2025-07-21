using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GameIt.Persistence.IntegrationTests
{
    public class GameItDatabaseContextTests
    {
        private readonly GameItDbContext _context;

        public GameItDatabaseContextTests()
        {
            // Initialize the GameItDbContext with an in-memory database for testing
            var options = new DbContextOptionsBuilder<GameItDbContext>()
                .UseInMemoryDatabase(databaseName: "GameItTestDatabase")
                .Options;
            _context = new GameItDbContext(options);
        }

        [Fact]
        public async Task Save_SetDateCreated()
        {
            // Arrange
            var game = new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Game",
                    Description = "Description",
                    Price = 19.99m,
                    IsFree = false,
                    IsFeatured = true,
                    CategoryId = Guid.NewGuid(),
                };
            // Act
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            // Assert
            game.CreatedAt.ShouldNotBeNull();
        }

        [Fact]
        public async Task Save_SetDateUpdated()
        {
            // Arrange
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Name = "Test Game",
                Description = "Description",
                Price = 19.99m,
                IsFree = false,
                IsFeatured = true,
                CategoryId = Guid.NewGuid(),
            };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            // Act
            game.Name = "Updated Test Game";
            await _context.SaveChangesAsync();
            // Assert
            game.UpdatedAt.ShouldNotBeNull();
        }
    }
}