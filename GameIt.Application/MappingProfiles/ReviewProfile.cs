using AutoMapper;
using GameIt.Application.Features.Review.Queries.GetReviewsByGame;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class ReviewProfile : Profile
{
    public ReviewProfile() 
    {
        // Map Review to ReviewListDto
        CreateMap<Review, ReviewListDto>()
            .ForMember(dest => dest.UserDisplayName,
                opt => opt.MapFrom(src => src.User.DisplayName)) 
            .ForMember(dest => dest.UserProfilePictureUrl,
                opt => opt.MapFrom(src => src.User.ProfilePictureUrl));
    }
}
