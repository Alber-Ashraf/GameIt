using System.Security.Claims;
using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;
using DomainGame = GameIt.Domain.Game;

namespace GameIt.Application.UnitTests.Features.Purchase.Commands;

public class CreatePurchaseCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IStripeService> _mockStripeService;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;

    public CreatePurchaseCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockStripeService = new Mock<IStripeService>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();
    }

    private void SetupAuthenticatedUser(string userId)
    {
        var claims = new List<Claim> { new("uid", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockContextAccessor.Setup(c => c.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task Handle_WhenClientTampersPrice_ComputesAuthoritativePriceServerSide()
    {
        // Arrange
        const string userId = "buyer-1";
        SetupAuthenticatedUser(userId);

        var gameId = Guid.NewGuid();
        var game = new DomainGame
        {
            Id = gameId,
            Name = "Cyberpunk RPG",
            Price = 59.99m,
            IsFree = false,
            Discount = null
        };

        var command = new CreatePurchaseCommand
        {
            GameId = gameId,
            AmountPaid = 0.01m // Tampered price by client
        };

        Domain.Purchase capturedPurchase = null!;
        _mockUow.Setup(u => u.Games.ExistsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Games.GetByIdWithDetailsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        _mockUow.Setup(u => u.Libraries.GetLibraryForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Library>());
        _mockUow.Setup(u => u.Purchases.CreateAsync(It.IsAny<Domain.Purchase>()))
            .Callback<Domain.Purchase>(p => capturedPurchase = p)
            .Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockStripeService.Setup(s => s.CreateCheckoutSessionAsync(
                It.IsAny<decimal>(),
                userId,
                gameId,
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CheckoutSessionResponse
            {
                CheckoutUrl = "https://stripe.test/checkout/session_123",
                SessionId = "sess_123",
                StripePaymentIntentId = "pi_12345"
            });

        var handler = new CreatePurchaseCommandHandler(
            _mockMapper.Object,
            _mockUow.Object,
            _mockStripeService.Object,
            _mockContextAccessor.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPurchase.ShouldNotBeNull();
        capturedPurchase.AmountPaid.ShouldBe(59.99m); // Server authoritative price
        capturedPurchase.Status.ShouldBe(PaymentStatus.Pending);
        result.StripeCheckoutUrl.ShouldBe("https://stripe.test/checkout/session_123");
        result.Status.ShouldBe(PaymentStatus.Pending);
    }

    [Fact]
    public async Task Handle_WhenGameHasActiveDiscount_AppliesDiscountAccurately()
    {
        // Arrange
        const string userId = "buyer-2";
        SetupAuthenticatedUser(userId);

        var gameId = Guid.NewGuid();
        var game = new DomainGame
        {
            Id = gameId,
            Name = "Strategy Sim",
            Price = 40.00m,
            IsFree = false,
            Discount = new Domain.Discount
            {
                Percentage = 25m, // 25% off of 40 = 30.00
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow.AddDays(5)
            }
        };

        var command = new CreatePurchaseCommand
        {
            GameId = gameId,
            AmountPaid = 0m
        };

        Domain.Purchase capturedPurchase = null!;
        _mockUow.Setup(u => u.Games.ExistsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Games.GetByIdWithDetailsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        _mockUow.Setup(u => u.Libraries.GetLibraryForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Library>());
        _mockUow.Setup(u => u.Purchases.CreateAsync(It.IsAny<Domain.Purchase>()))
            .Callback<Domain.Purchase>(p => capturedPurchase = p)
            .Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockStripeService.Setup(s => s.CreateCheckoutSessionAsync(
                30.00m,
                userId,
                gameId,
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CheckoutSessionResponse
            {
                CheckoutUrl = "https://stripe.test/checkout/session_discounted",
                SessionId = "sess_discounted",
                StripePaymentIntentId = "pi_discounted"
            });

        var handler = new CreatePurchaseCommandHandler(
            _mockMapper.Object,
            _mockUow.Object,
            _mockStripeService.Object,
            _mockContextAccessor.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPurchase.ShouldNotBeNull();
        capturedPurchase.AmountPaid.ShouldBe(30.00m);
        result.StripeCheckoutUrl.ShouldBe("https://stripe.test/checkout/session_discounted");
    }

    [Fact]
    public async Task Handle_WhenGameIsFree_SetsCompletedStatusAndAddsToLibraryAutomatically()
    {
        // Arrange
        const string userId = "buyer-free";
        SetupAuthenticatedUser(userId);

        var gameId = Guid.NewGuid();
        var game = new DomainGame
        {
            Id = gameId,
            Name = "Free Battle Royale",
            Price = 0.00m,
            IsFree = true
        };

        var command = new CreatePurchaseCommand
        {
            GameId = gameId,
            AmountPaid = 99.99m // Client sent wrong amount for free game
        };

        Domain.Purchase capturedPurchase = null!;
        Domain.Library capturedLibrary = null!;

        _mockUow.Setup(u => u.Games.ExistsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Games.GetByIdWithDetailsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        _mockUow.Setup(u => u.Libraries.GetLibraryForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Library>());
        _mockUow.Setup(u => u.Purchases.CreateAsync(It.IsAny<Domain.Purchase>()))
            .Callback<Domain.Purchase>(p => capturedPurchase = p)
            .Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.Libraries.CreateAsync(It.IsAny<Domain.Library>()))
            .Callback<Domain.Library>(l => capturedLibrary = l)
            .Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreatePurchaseCommandHandler(
            _mockMapper.Object,
            _mockUow.Object,
            _mockStripeService.Object,
            _mockContextAccessor.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(PaymentStatus.Succeeded);
        result.StripeCheckoutUrl.ShouldBeNull();
        capturedPurchase.ShouldNotBeNull();
        capturedPurchase.AmountPaid.ShouldBe(0.00m);
        capturedPurchase.Status.ShouldBe(PaymentStatus.Succeeded);

        capturedLibrary.ShouldNotBeNull();
        capturedLibrary.GameId.ShouldBe(gameId);
        capturedLibrary.UserId.ShouldBe(userId);

        _mockStripeService.Verify(s => s.CreateCheckoutSessionAsync(
            It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyOwnsGame_ThrowsBadRequestException()
    {
        // Arrange
        const string userId = "duplicate-buyer";
        SetupAuthenticatedUser(userId);

        var gameId = Guid.NewGuid();
        var game = new DomainGame
        {
            Id = gameId,
            Name = "Existing Game",
            Price = 20.00m
        };

        _mockUow.Setup(u => u.Games.ExistsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Games.GetByIdWithDetailsAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        _mockUow.Setup(u => u.Libraries.GetLibraryForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Library>
            {
                new Domain.Library { GameId = gameId, UserId = userId }
            });

        var handler = new CreatePurchaseCommandHandler(
            _mockMapper.Object,
            _mockUow.Object,
            _mockStripeService.Object,
            _mockContextAccessor.Object);

        var command = new CreatePurchaseCommand { GameId = gameId };

        // Act & Assert
        var ex = await Should.ThrowAsync<BadRequestException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });

        ex.Message.ShouldContain("already own");
    }
}
