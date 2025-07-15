using AutoMapper;
using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Application.Features.Purchase.Commands.RefundPurchase;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class PurchaseProfile : Profile
{
    public PurchaseProfile()
    {
        // Create Command -> Entity
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

        // Refund Command -> Entity
        CreateMap<RefundPurchaseCommand, Purchase>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.Game,
                opt => opt.Ignore())
            .ForMember(dest => dest.User,
                opt => opt.Ignore())
            .ForMember(dest => dest.RefundDate, 
                opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Entity -> Refund Response
        CreateMap<Purchase, RefundResponse>()
            .ForMember(dest => dest.AmountRefunded,
                opt => opt.MapFrom(src => src.AmountPaid * -1))
            .ForMember(dest => dest.Currency,
                opt => opt.MapFrom(src => src.Currency))
            .ForMember(dest => dest.RefundDate,
                opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.RefundId,
                opt => opt.MapFrom(src => Guid.NewGuid()));
    }
}
