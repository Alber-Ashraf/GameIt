using AutoMapper;
using GameIt.BlazorUI.Models.Auth;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.MappingProfiles;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        // Mapping for Auth
        CreateMap<LoginVM, AuthRequest>().ReverseMap();
        CreateMap<RegisterVM, RegistrationRequest>().ReverseMap();

        // Mapping for Game
        CreateMap<GamesListVM, GamesListDto>().ReverseMap();
        CreateMap<GameDetailsVM, GamesListDto>().ReverseMap();
        CreateMap<CreateGameCommand, GameDetailsVM>().ReverseMap();
    }
}
