using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Review.Commands.UpdateReview;

public class UpdateReviewCommandhandler : IRequestHandler<UpdateReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    public UpdateReviewCommandhandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }
    public async Task<Unit> Handle(UpdateReviewCommand request, CancellationToken token)
    {
        // Validate
        var validator = new UpdateReviewCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Review", validationResult);
        // Fetch and verify
        var existingReview = await _unitOfWork.Reviews.GetByIdAsync(request.Id);

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Validate if the review exists and belongs to the user
        if (existingReview == null || existingReview.UserId != userId)
            throw new NotFoundException("Review not found or access denied.");

        // AutoMapper update (preserves unchanged values)
        _mapper.Map(request, existingReview);
        // Update the review in the repository
        _unitOfWork.Reviews.Update(existingReview);
        // Save
        await _unitOfWork.SaveChangesAsync(token);
        // Return Unit
        return Unit.Value;
    }
}
