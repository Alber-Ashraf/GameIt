using AutoMapper;
using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class PurchaseProfile : Profile
{
    public PurchaseProfile()
    {
        // Command -> Entity
        CreateMap<CreatePurchaseCommand, Purchase>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentStatus,
                opt => opt.MapFrom((src, dest, member, context) =>
                    context.Items["PaymentStatus"]))
            .ForMember(dest => dest.PurchaseDate,
                opt => opt.MapFrom((src, dest, member, context) =>
                    context.Items["PurchaseDate"]));

        // Entity -> Response
        CreateMap<Purchase, PurchaseResponse>();
    }
}
