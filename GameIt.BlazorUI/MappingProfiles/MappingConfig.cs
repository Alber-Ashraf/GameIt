using AutoMapper;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.MappingProfiles;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<GamesListVM, GamesListDto>().ReverseMap();
        CreateMap<GameDetailsVM, GamesListDto>().ReverseMap();
        CreateMap<CreateGameCommand, GameDetailsVM>().ReverseMap();
    }
}
