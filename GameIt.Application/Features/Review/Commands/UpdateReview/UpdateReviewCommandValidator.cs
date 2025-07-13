using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Review.Commands.UpdateReview;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Review ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be 1-5 if provided.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comment))
            .WithMessage("Comment cannot exceed 1000 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
