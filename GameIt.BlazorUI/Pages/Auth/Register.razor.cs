using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Auth;
using Microsoft.AspNetCore.Components;

namespace GameIt.BlazorUI.Pages.Auth;

public partial class Register
{

    public RegisterVM Model { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public string Message { get; set; }

    [Inject]
    private IAuthenticationService AuthenticationService { get; set; }

    protected override void OnInitialized()
    {
        Model = new RegisterVM();
    }

    protected async Task HandleRegister()
    {
        var result = await AuthenticationService.RegisterAsync(Model);

        if (result)
        {
            NavigationManager.NavigateTo("/");
        }
        Message = "Something went wrong, please try again.";
    }
}
