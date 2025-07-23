using Blazored.LocalStorage;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class PurchaseService : BaseHttpService, IPurchaseService
{
    public PurchaseService(IClient client, ILocalStorageService localStorage) : base(client, localStorage)
    {

    }
}
