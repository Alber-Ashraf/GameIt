using AutoMapper;
using GameIt.Application.Features.Category.Commands.CreateCategory;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile() 
    {
        // Commands Mapping
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
