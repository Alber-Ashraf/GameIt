using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using Moq;

namespace GameIt.Application.UnitTests.Mocks;

public class MockGameRepository
{
    private static readonly List<Game> _games = new()
    {
        new Game
        {
            Id = Guid.NewGuid(),
            Name = "Test Game 1",
            Description = "Description 1",
            Price = 19.99m,
            IsFree = false,
            IsFeatured = true,
            CategoryId = Guid.NewGuid(),
        },
        new Game
        {
            Id = Guid.NewGuid(),
            Name = "Test Game 2",
            Description = "Description 2",
            Price = 29.99m,
            IsFree = false,
            IsFeatured = false,
            CategoryId = Guid.NewGuid(),
        },
        new Game
        {
            Id = Guid.NewGuid(),
            Name = "Test Game 3",
            Description = "Description 3",
            Price = 0.00m, 
            IsFree = true,
            IsFeatured = false,
            CategoryId = Guid.NewGuid(),
        }
    };

    public static Mock<IUnitOfWork> GetMockGameRepository()
    {
        var mockRepo = new Mock<IUnitOfWork>();

        // Get All Games test
        mockRepo.Setup(repo => repo.Games.GetAllWithCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_games);

        return mockRepo;
    }
}
