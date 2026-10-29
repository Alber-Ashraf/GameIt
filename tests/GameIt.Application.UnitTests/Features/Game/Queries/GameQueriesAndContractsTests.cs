using AutoMapper;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Features.Game.Queries.GetFeaturedGames;
using GameIt.Application.Features.Game.Queries.GetGamesByCategory;
using GameIt.Application.Features.Game.Queries.GetSimilarGames;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.MappingProfiles;
using GameIt.Contracts.Games;
using GameIt.Domain;
using Moq;
using Shouldly;
using Xunit;

namespace GameIt.Application.UnitTests.Features.Game.Queries;

public class GameQueriesAndContractsTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _mockUow;

    public GameQueriesAndContractsTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new GameProfile()));
        _mapper = config.CreateMapper();
        _mockUow = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task GetAllGamesList_WhenNoGamesExist_ReturnsEmptyListWithoutThrowing()
    {
        // Arrange
        _mockUow.Setup(u => u.Games.GetAllWithCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Game>());

        var handler = new GetAllGamesListQueryHandler(_mapper, _mockUow.Object);

        // Act
        var result = await handler.Handle(new GetAllGamesListQuery(), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFeaturedGames_WhenNoFeaturedGamesExist_ReturnsEmptyListWithoutThrowing()
    {
        // Arrange
        _mockUow.Setup(u => u.Games.GetFeaturedGamesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Game>());

        var handler = new GetFeaturedGamesQueryHandler(_mapper, _mockUow.Object);

        // Act
        var result = await handler.Handle(new GetFeaturedGamesQuery(5), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetSimilarGames_WhenNoSimilarGamesExist_ReturnsEmptyListWithoutThrowing()
    {
        // Arrange
        _mockUow.Setup(u => u.Games.GetSimilarGamesAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Game>());

        var handler = new GetSimilarGamesQueryHandler(_mapper, _mockUow.Object);

        // Act
        var result = await handler.Handle(new GetSimilarGamesQuery(Guid.NewGuid(), 4), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetGamesByCategory_WhenCategoryExistsButHasNoGames_ReturnsEmptyListWithoutThrowing()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockUow.Setup(u => u.Categories.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Games.GetGamesByCategoryAsync(categoryId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Game>());

        var handler = new GetGamesByCategoryQueryHandler(_mapper, _mockUow.Object);

        // Act
        var result = await handler.Handle(new GetGamesByCategoryQuery(categoryId, 10), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(59.99, false, null, 59.99)]
    [InlineData(59.99, true, null, 0.00)]
    [InlineData(40.00, false, 25, 30.00)]
    [InlineData(19.99, false, 50, 10.00)]
    public void GameSummaryDto_FinalPrice_ComputesExpectedValue(
        decimal price,
        bool isFree,
        int? discount,
        decimal expectedFinalPrice)
    {
        var dto = new GameSummaryDto
        {
            Price = price,
            IsFree = isFree,
            Discount = discount
        };

        dto.FinalPrice.ShouldBe(expectedFinalPrice);
    }
}
