﻿using MediatR;

namespace GameIt.Application.Features.Wishlist.Commands.ClearWishlist;

public class ClearWishlistCommand : IRequest<Unit>
{
    public string UserId { get; set; }
}