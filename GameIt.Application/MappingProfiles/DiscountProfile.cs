using AutoMapper;
using GameIt.Application.Features.Discount.Commands.CreateDiscount;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        // Map from CreateDiscountCommand to Discount entity
        CreateMap<CreateDiscountCommand, Discount>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}
