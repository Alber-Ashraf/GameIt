using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Review.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;

    public DeleteReviewCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteReviewCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Review", validationResult);

        var currentUserId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");
        var isAdmin = _contextAccessor.HttpContext?.User?.IsInRole("Administrator") ?? false;

        if (string.IsNullOrEmpty(currentUserId))
            throw new UnauthorizedAccessException("User must be authenticated.");

        var existingReview = await _unitOfWork.Reviews.GetByIdAsync(request.Id);
        if (existingReview == null)
            throw new NotFoundException(nameof(Domain.Review), request.Id);

        if (existingReview.UserId != currentUserId && !isAdmin)
            throw new UnauthorizedAccessException("You are not authorized to delete this review.");

        _unitOfWork.Reviews.Delete(existingReview);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
