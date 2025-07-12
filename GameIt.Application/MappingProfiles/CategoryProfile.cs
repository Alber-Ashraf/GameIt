using AutoMapper;
using GameIt.Application.Features.Category.Commands.CreateCategory;
using GameIt.Application.Features.Category.Commands.UpdateCategory;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile() 
    {
        // Commands Mapping
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore());
    }
}
