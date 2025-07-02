using AutoMapper;
using GameIt.Application.Features.Game.Commands.CreateGame;
using GameIt.Application.Features.Game.Queries.GetAllGameDetails;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Features.Review.Queries;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            // Mapping configurations for Game entity to DTOs
            CreateMap<Game, GamesListDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Game, GameDetailsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null));

            CreateMap<CreateGameCommand, Game>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // Mapping configurations for Review entity to DTOs
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserDisplayName, opt => opt.MapFrom(src => src.User.DisplayName))
                .ForMember(dest => dest.UserProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePictureUrl));
            
            // 
        }
    }
}
