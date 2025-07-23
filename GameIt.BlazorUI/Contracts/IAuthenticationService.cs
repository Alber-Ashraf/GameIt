namespace GameIt.BlazorUI.Contracts;

public interface IAuthenticationService
{
    Task<bool> AuthenticationAsync(string email, string password);
    Task<bool> RegisterAsync(string FirstName, string LastName, string ProfileImageUrl, string userName, string email, string password);
    Task Logout();
}
