using AutoMapper;
using GameIt.Application.Features.Discount.Commands.CreateDiscount;
using GameIt.Application.Features.Discount.Commands.UpdateDiscount;
using GameIt.Application.Features.Discount.Queries.GetActiveDiscounts;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        // Map from Discount entity to GetDiscountResponse
        CreateMap<Discount, ActiveDiscountDto>()
            .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Game.Name))
            .ForMember(dest => dest.GameImageUrl, opt => opt.MapFrom(src => src.Game.ImageUrl))
            .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.Game.Price));

        // Map from CreateDiscountCommand to Discount entity
        CreateMap<CreateDiscountCommand, Discount>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // Map from UpdateDiscountCommand to Discount entity
        CreateMap<UpdateDiscountCommand, Discount>()
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}
