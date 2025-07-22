using AutoMapper;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.MappingProfiles;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<GameDetailsDto, GameVM>().ReverseMap();
        CreateMap<CreateGameCommand, GameVM>().ReverseMap();
    }
}
