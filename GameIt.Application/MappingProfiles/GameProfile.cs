using AutoMapper;
using GameIt.Application.Features.Game.Commands.CreateGame;
using GameIt.Application.Features.Game.Commands.UpdateGame;
using GameIt.Application.Features.Game.Queries.GetGameDetails;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Domain;
using GameIt.Application.Features.Review.Queries.GetReviewsByGame;

namespace GameIt.Application.MappingProfiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        // Mapping configurations for Game entity to DTOs and commands
        // GamesListDto Mapping
        CreateMap<Game, GamesListDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.IsOnSale, opt => opt.MapFrom(src => src.Discount != null))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount != null ? (int?)src.Discount.Percentage : null))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null));

        // GameDetailsDto Mapping
        CreateMap<Game, GameDetailsDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.Discount != null ? (decimal?)src.Discount.Percentage : null))
            .ForMember(dest => dest.DiscountStartDate, opt => opt.MapFrom(src => src.Discount != null ? (DateTime?)src.Discount.StartDate : null))
            .ForMember(dest => dest.DiscountEndDate, opt => opt.MapFrom(src => src.Discount != null ? (DateTime?)src.Discount.EndDate : null))
            .ForMember(dest => dest.IsDiscounted, opt => opt.MapFrom(src => src.Discount != null && src.Discount.IsActive))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src =>
                src.Reviews.Select(r => new ReviewListDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserDisplayName = r.User.DisplayName
                }).ToList()));

        // Commands Mapping
        CreateMap<CreateGameCommand, Game>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdateGameCommand, Game>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Purchases, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore());            
    }
}
