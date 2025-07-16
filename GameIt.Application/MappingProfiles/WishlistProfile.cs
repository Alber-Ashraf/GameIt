using AutoMapper;
using GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class WishlistProfile : Profile
{
    public WishlistProfile()
    {
        // Entity -> List DTO
        CreateMap<Wishlist, WishlistListDto>()
            .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Game.ImageUrl))
            .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
