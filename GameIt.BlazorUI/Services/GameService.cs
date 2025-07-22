using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class GameService : BaseHttpService, IGameService
{
    public GameService(IClient client) : base(client)
    {

    }
}