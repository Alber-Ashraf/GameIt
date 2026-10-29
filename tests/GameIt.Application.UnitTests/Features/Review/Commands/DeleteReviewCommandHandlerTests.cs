using System.Security.Claims;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Review.Commands.DeleteReview;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace GameIt.Application.UnitTests.Features.Review.Commands;

public class DeleteReviewCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;

    public DeleteReviewCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();
    }

    private void SetupUser(string userId, string? role = null)
    {
        var claims = new List<Claim> { new("uid", userId) };
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _mockContextAccessor.Setup(c => c.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task Handle_WhenUserIsOwner_DeletesReviewSuccessfully()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        const string ownerId = "user-123";
        SetupUser(ownerId, "User");

        var review = new Domain.Review
        {
            Id = reviewId,
            UserId = ownerId,
            GameId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Masterpiece!"
        };

        _mockUow.Setup(u => u.Reviews.ExistsAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Reviews.GetByIdAsync(reviewId))
            .ReturnsAsync(review);
        _mockUow.Setup(u => u.Reviews.Delete(It.IsAny<Domain.Review>()));
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteReviewCommandHandler(_mockUow.Object, _mockContextAccessor.Object);
        var command = new DeleteReviewCommand { Id = reviewId };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUow.Verify(u => u.Reviews.Delete(review), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsAdministrator_DeletesReviewEvenIfNotOwner()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        const string ownerId = "user-123";
        const string adminId = "admin-999";
        SetupUser(adminId, "Administrator");

        var review = new Domain.Review
        {
            Id = reviewId,
            UserId = ownerId,
            GameId = Guid.NewGuid(),
            Rating = 1,
            Comment = "Spam review."
        };

        _mockUow.Setup(u => u.Reviews.ExistsAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Reviews.GetByIdAsync(reviewId))
            .ReturnsAsync(review);
        _mockUow.Setup(u => u.Reviews.Delete(It.IsAny<Domain.Review>()));
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteReviewCommandHandler(_mockUow.Object, _mockContextAccessor.Object);
        var command = new DeleteReviewCommand { Id = reviewId };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUow.Verify(u => u.Reviews.Delete(review), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerAndNotAdmin_ThrowsUnauthorizedAccessExceptionForIdorProtection()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        const string ownerId = "user-123";
        const string attackerId = "attacker-456";
        SetupUser(attackerId, "User");

        var review = new Domain.Review
        {
            Id = reviewId,
            UserId = ownerId,
            GameId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Legitimate review."
        };

        _mockUow.Setup(u => u.Reviews.ExistsAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUow.Setup(u => u.Reviews.GetByIdAsync(reviewId))
            .ReturnsAsync(review);

        var handler = new DeleteReviewCommandHandler(_mockUow.Object, _mockContextAccessor.Object);
        var command = new DeleteReviewCommand { Id = reviewId };

        // Act & Assert
        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });

        _mockUow.Verify(u => u.Reviews.Delete(It.IsAny<Domain.Review>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        _mockUow.Setup(u => u.Reviews.ExistsAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal() };
        _mockContextAccessor.Setup(c => c.HttpContext).Returns(httpContext);

        var handler = new DeleteReviewCommandHandler(_mockUow.Object, _mockContextAccessor.Object);
        var command = new DeleteReviewCommand { Id = reviewId };

        // Act & Assert
        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }
}
