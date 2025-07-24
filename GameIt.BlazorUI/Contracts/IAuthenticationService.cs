using GameIt.BlazorUI.Models.Auth;

namespace GameIt.BlazorUI.Contracts;

public interface IAuthenticationService
{
    Task<bool> AuthenticationAsync(LoginVM loginVM);
    Task<bool> RegisterAsync(RegisterVM registerVM);
    Task Logout();
}
