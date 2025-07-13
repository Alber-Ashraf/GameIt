using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Review.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new DeleteReviewCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Review", validationResult);

        // Fetch existing Review
        var existingReview = await _unitOfWork.Reviews.GetByIdAsync(request.Id);
        if (existingReview == null)
            throw new NotFoundException(nameof(Review), request.Id);

        // Validate if the review exists and belongs to the user

        /*
        if (existingReview == null || existingReview.UserId != request.UserId)
            throw new NotFoundException("Review not found or access denied.");
        */

        // Delete the Review entity from the repository
        _unitOfWork.Reviews.Delete(existingReview);

        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();

        // Return Unit.Value to indicate successful completion
        return Unit.Value;
    }
}