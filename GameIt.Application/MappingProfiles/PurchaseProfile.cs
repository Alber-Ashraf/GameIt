using AutoMapper;
using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Application.Features.Purchase.Commands.RefundPurchase;
using GameIt.Application.Features.Purchase.Queries.GetUserPurchase;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using Stripe;

namespace GameIt.Application.MappingProfiles;

public class PurchaseProfile : Profile
{
    public PurchaseProfile()
    {
        // Entity -> List DTO
        CreateMap<Purchase, PurchaseListDto>()
            .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Game.Name))
            .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.Game.Price));


        // Create Command -> Entity
        CreateMap<CreatePurchaseCommand, Purchase>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseDate,
                opt => opt.MapFrom((src, dest, member, context) =>
                    context.Items["PurchaseDate"]));

        // Entity -> Response
        CreateMap<Purchase, PurchaseResult>();

        // Refund Command -> Entity
        CreateMap<RefundPurchaseCommand, Purchase>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.Game,
                opt => opt.Ignore());

        // Entity -> Refund Response
        CreateMap<Refund, RefundResult>()
            .ForMember(dest => dest.Success, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.RefundId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.RefundDate, opt => opt.MapFrom(src => src.Created));
    }
}
