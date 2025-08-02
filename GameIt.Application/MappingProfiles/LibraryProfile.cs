using AutoMapper;
using GameIt.Application.Features.Library.Queries.GetUserLibrary;
using GameIt.Domain;

namespace GameIt.Application.MappingProfiles;

public class LibraryProfile : Profile
{
    public LibraryProfile()
    {
        CreateMap<Library, LibraryListDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Game.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Game.Name))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Game.Description))
                    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Game.ImageUrl))
                    .ForMember(dest => dest.FileSizeInBytes, opt => opt.MapFrom(src => src.Game.FileSizeInBytes))
                    .ForMember(dest => dest.DownloadLink, opt => opt.MapFrom(src => src.Game.DownloadLink))
                    .ForMember(dest => dest.SystemRequirements, opt => opt.MapFrom(src => src.Game.SystemRequirements))
                    .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Game.ReleaseDate))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Game.Category.Name))
                    .ForMember(dest => dest.PurchasedAt, opt => opt.MapFrom(src => src.PurchasedAt));
    }
}
