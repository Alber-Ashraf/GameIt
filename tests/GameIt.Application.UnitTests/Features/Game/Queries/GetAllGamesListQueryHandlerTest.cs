using AutoMapper;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.MappingProfiles;
using GameIt.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace GameIt.Application.UnitTests.Features.Game.Queries;

public class GetAllGamesListQueryHandlerTest
{
    private IMapper _mapper;
    private readonly Mock<IUnitOfWork> _mockGameRepository;
    public GetAllGamesListQueryHandlerTest()
    {
        _mockGameRepository = MockGameRepository.GetMockGameRepository();

        // Setup the mock repository to return a predefined list of games
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new GameProfile());
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllGamesList_WhenGamesExist()
    {
        // Arrange
        var handler = new GetAllGamesListQueryHandler(_mapper, _mockGameRepository.Object);
        var query = new GetAllGamesListQuery();
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Shouldly assertions
        result.ShouldBeOfType<List<GamesListDto>>();
        result.Count.ShouldBeGreaterThan(0);
    }
}
