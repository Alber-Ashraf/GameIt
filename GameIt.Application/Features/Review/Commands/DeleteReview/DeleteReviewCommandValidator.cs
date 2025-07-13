using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Review.Commands.DeleteReview;

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Review ID is required.")
            .MustAsync(ReviewExists).WithMessage("Review does not exist.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }

    private async Task<bool> ReviewExists(Guid reviewId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Reviews.ExistsAsync(reviewId, cancellationToken);
    }
}