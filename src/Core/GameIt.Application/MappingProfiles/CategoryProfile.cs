using AutoMapper;
using GameIt.Application.Features.Category.Commands.CreateCategory;
using GameIt.Application.Features.Category.Commands.UpdateCategory;
using GameIt.Application.Features.Category.Queries.GetAllGameLists;
using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile() 
    {
        // Mapping configurations for Game entity to DTOs and commands
        // CategoriesListDto Mapping
        CreateMap<Category, CategoriesListDto>();

        // GameDetailsDto Mapping
        CreateMap<Category, CategoryDetailsDto>()
            .ForMember(
                dest => dest.Games,
                opt => opt.MapFrom(src => src.Games ?? Enumerable.Empty<Game>())
            );

        // Commands Mapping
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore());
    }
}
