using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Review.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public CreateReviewCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }
    public async Task<Guid> Handle(
        CreateReviewCommand request,
        CancellationToken token)
    {
        // Validate the request 
        var validator = new CreateReviewCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Review", validationResult);

        // Map the request to a Review entity
        var ReviewToCreate = _mapper.Map<Domain.Review>(request);

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Set the User Id
        ReviewToCreate.UserId = userId;

        // Create the Review in the repository
        await _unitOfWork.Reviews.CreateAsync(ReviewToCreate);
            
        // Save changes
        await _unitOfWork.SaveChangesAsync(token);

        // Return the new Review's ID
        return ReviewToCreate.Id;
    }
}
