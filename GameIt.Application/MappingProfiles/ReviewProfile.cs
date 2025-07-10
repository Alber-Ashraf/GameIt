using AutoMapper;
using GameIt.Application.Features.Review.Queries;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class ReviewProfile : Profile
{
    public ReviewProfile() 
    {
        // Mapping configurations for Review entity to DTOs
        CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserDisplayName, opt => opt.MapFrom(src => src.User.DisplayName))
                .ForMember(dest => dest.UserProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePictureUrl));
    }
}
