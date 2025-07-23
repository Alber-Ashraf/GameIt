using AutoMapper;
using GameIt.Application.Features.Review.Commands.CreateReview;
using GameIt.Application.Features.Review.Commands.UpdateReview;
using GameIt.Application.Features.Review.Queries.GetReviewsByGame;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class ReviewProfile : Profile
{
    public ReviewProfile() 
    {
        // Map Review to ReviewListDto
        CreateMap<Review, ReviewListDto>();

        // Map Command to Review
        CreateMap<CreateReviewCommand, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId));

        // Map UpdateReviewCommand to Review
        CreateMap<UpdateReviewCommand, Review>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
