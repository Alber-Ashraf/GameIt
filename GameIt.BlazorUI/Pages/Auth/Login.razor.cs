using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Auth;
using Microsoft.AspNetCore.Components;

namespace GameIt.BlazorUI.Pages.Auth;

public partial class Login
{
    public LoginVM Model { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }
    public string Message { get; set; }

    [Inject]
    private IAuthenticationService AuthenticationService { get; set; }

    public Login()
    {

    }

    protected override void OnInitialized()
    {
        Model = new LoginVM();
    }

    protected async Task HandleLogin()
    {
        if (await AuthenticationService.AuthenticationAsync(Model))
        {
            NavigationManager.NavigateTo("/");
        }
        Message = "Username/password combination unknown";
    }
}
