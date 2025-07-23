using Blazored.LocalStorage;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Providers;
using GameIt.BlazorUI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace GameIt.BlazorUI.Services;

public class AuthenticationService : BaseHttpService, IAuthenticationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    public AuthenticationService(IClient client,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider) :
        base(client, localStorage)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> AuthenticationAsync(string email, string password)
    {
        try
        {
            AuthRequest authRequest = new AuthRequest
            {
                Email = email,
                Password = password
            };

            var authResponse = await _client.LoginAsync(authRequest);
            if (authResponse.Token != string.Empty)
            {
                await _localStorage.SetItemAsync("token", authResponse.Token);

                // Set claims in Blazor and login state
                await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();

                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task Logout()
    {
        // remove claims in Blazor and invalidate login state
        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
    }

    public async Task<bool> RegisterAsync(string FirstName, string LastName, string ProfilePictureUrl, string userName, string email, string password)
    {
        try
        {
            RegistrationRequest registerRequest = new RegistrationRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                ProfilePictureUrl = ProfilePictureUrl,
                UserName = userName,
                Email = email,
                Password = password
            };
            var response = await _client.RegisterAsync(registerRequest);

            if (!string.IsNullOrEmpty(response.UserId))
            {
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
