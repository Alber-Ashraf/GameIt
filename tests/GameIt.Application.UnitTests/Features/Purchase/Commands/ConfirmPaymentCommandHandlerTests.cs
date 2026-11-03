using System.Security.Claims;
using AutoMapper;
using GameIt.Application.Features.Purchase.Commands.ConfirmPayment;
using GameIt.Application.Features.Purchase.Commands.VerifyPurchase;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Stripe.Checkout;
using Xunit;

namespace GameIt.Application.UnitTests.Features.Purchase.Commands;

public class ConfirmPaymentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IStripeService> _mockStripeService;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
    private readonly Mock<ITransactionScope> _mockTransaction;

    public ConfirmPaymentCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockStripeService = new Mock<IStripeService>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();
        _mockTransaction = new Mock<ITransactionScope>();

        _mockUow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);
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
    public async Task ConfirmPayment_WhenSessionPaid_MarksSucceededAndAddsToLibrary()
    {
        // Arrange
        const string userId = "buyer-123";
        SetupAuthenticatedUser(userId);

        var purchaseId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var purchase = new Domain.Purchase
        {
            Id = purchaseId,
            UserId = userId,
            GameId = gameId,
            Status = PaymentStatus.Pending,
            AmountPaid = 59.99m
        };

        var game = new Domain.Game
        {
            Id = gameId,
            Name = "Elden Ring"
        };

        var session = new Session
        {
            Id = "cs_test_12345",
            PaymentStatus = "paid",
            PaymentIntentId = "pi_12345",
            Metadata = new Dictionary<string, string>
            {
                { "purchase_id", purchaseId.ToString() },
                { "userId", userId }
            }
        };

        _mockStripeService.Setup(s => s.GetCheckoutSessionAsync("cs_test_12345", It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _mockUow.Setup(u => u.Purchases.GetByIdAsync(purchaseId))
            .ReturnsAsync(purchase);
        _mockUow.Setup(u => u.Games.GetByIdAsync(gameId))
            .ReturnsAsync(game);
        _mockUow.Setup(u => u.Libraries.GetLibraryForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Library>());

        Domain.Library? capturedLibrary = null;
        _mockUow.Setup(u => u.Libraries.CreateAsync(It.IsAny<Domain.Library>()))
            .Callback<Domain.Library>(l => capturedLibrary = l)
            .Returns(Task.CompletedTask);

        var handler = new ConfirmPaymentCommandHandler(
            _mockUow.Object,
            _mockStripeService.Object,
            _mockContextAccessor.Object);

        // Act
        var result = await handler.Handle(new ConfirmPaymentCommand("cs_test_12345"), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(PaymentStatus.Succeeded);
        result.GameName.ShouldBe("Elden Ring");

        purchase.Status.ShouldBe(PaymentStatus.Succeeded);
        purchase.StripePaymentIntentId.ShouldBe("pi_12345");

        capturedLibrary.ShouldNotBeNull();
        capturedLibrary.UserId.ShouldBe(userId);
        capturedLibrary.GameId.ShouldBe(gameId);

        _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task VerifyPurchase_WhenExistingPendingPurchaseIsPaidInStripe_FulfillsSuccessfully()
    {
        // Arrange
        const string userId = "buyer-123";
        SetupAuthenticatedUser(userId);

        var purchaseId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var purchase = new Domain.Purchase
        {
            Id = purchaseId,
            UserId = userId,
            GameId = gameId,
            Status = PaymentStatus.Pending,
            StripePaymentIntentId = "cs_test_999",
            AmountPaid = 29.99m
        };

        var game = new Domain.Game
        {
            Id = gameId,
            Name = "Cyberpunk 2077"
        };

        var session = new Session
        {
            Id = "cs_test_999",
            PaymentStatus = "paid",
            PaymentIntentId = "pi_999",
            Metadata = new Dictionary<string, string>
            {
                { "purchase_id", purchaseId.ToString() }
            }
        };

        _mockUow.Setup(u => u.Purchases.GetByIdAsync(purchaseId))
            .ReturnsAsync(purchase);
        _mockUow.Setup(u => u.Games.GetByIdAsync(gameId))
            .ReturnsAsync(game);
        _mockUow.Setup(u => u.Libraries.GetLibraryForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Library>());

        _mockStripeService.Setup(s => s.GetCheckoutSessionAsync("cs_test_999", It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var handler = new VerifyPurchaseCommandHandler(
            _mockUow.Object,
            _mockStripeService.Object,
            _mockContextAccessor.Object);

        // Act
        var result = await handler.Handle(new VerifyPurchaseCommand(purchaseId), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(PaymentStatus.Succeeded);
        purchase.Status.ShouldBe(PaymentStatus.Succeeded);

        _mockUow.Verify(u => u.Libraries.CreateAsync(It.Is<Domain.Library>(l => l.GameId == gameId && l.UserId == userId)), Times.Once());
        _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}
