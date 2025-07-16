using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public AddToWishlistCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new AddToWishlistCommandValidator(_unitOfWork);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Wishlist Item", validationResult);

        var exists = await _unitOfWork.Wishlists.IsGameInWishlistAsync(request.UserId, request.GameId);

        if (exists)
            throw new BadRequestException("This game is already in your wishlist.");


        // Map the request to a Wishlist entity
        var wishlistItem = _mapper.Map<Domain.Wishlist>(request, opt =>
        {
            opt.Items["CreatedAt"] = DateTime.UtcNow;
        });

        // Add the item to the wishlist
        await _unitOfWork.Wishlists.CreateAsync(wishlistItem);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return the new Wishlist item's ID
        return wishlistItem.Id;
    }
}
