using Blazored.LocalStorage;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class CategoryService : BaseHttpService, ICategoryService
{
    public CategoryService(IClient client, ILocalStorageService localStorage) : base(client, localStorage)
    {

    }
}
