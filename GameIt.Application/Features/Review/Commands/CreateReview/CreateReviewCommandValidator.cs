﻿using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Review.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateReviewCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");

        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("Game ID is required.")
            .MustAsync(GameExists).WithMessage("Game does not exist.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
    private async Task<bool> GameExists(Guid gameId, CancellationToken token)
    {
        return await _unitOfWork.Games.ExistsAsync(gameId, token);
    }
}
