using Blazored.LocalStorage;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class WishlistService : BaseHttpService, IWishlistService
{
    public WishlistService(IClient client, ILocalStorageService localStorage) : base(client, localStorage)
    {

    }
}