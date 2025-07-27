using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    public AddToWishlistCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<Guid> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new AddToWishlistCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Wishlist Item", validationResult);

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        var exists = await _unitOfWork.Wishlists.IsGameInWishlistAsync(userId, request.GameId);

        if (exists)
            throw new BadRequestException("This game is already in your wishlist.");

        // Map the request to a Wishlist entity
        var wishlistItem = _mapper.Map<Domain.Wishlist>(request, opt =>
        {
            opt.Items["CreatedAt"] = DateTime.UtcNow;
        });

        // Set the UserId for the Wishlist item
        wishlistItem.UserId = userId;

        // Add the item to the wishlist
        await _unitOfWork.Wishlists.CreateAsync(wishlistItem);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return the new Wishlist item's ID
        return wishlistItem.Id;
    }
}
