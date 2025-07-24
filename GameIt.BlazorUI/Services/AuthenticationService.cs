using AutoMapper;
using Blazored.LocalStorage;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Auth;
using GameIt.BlazorUI.Providers;
using GameIt.BlazorUI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace GameIt.BlazorUI.Services;

public class AuthenticationService : BaseHttpService, IAuthenticationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IMapper _mapper;
    public AuthenticationService(IClient client,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider,
        IMapper mapper) :
        base(client, localStorage)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _mapper = mapper;
    }

    public async Task<bool> AuthenticationAsync(LoginVM loginVM)
    {
        try
        {
            AuthRequest authRequest = _mapper.Map<AuthRequest>(loginVM);

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

    public async Task<bool> RegisterAsync(RegisterVM registerVM)
    {
        try
        {
            RegistrationRequest registerRequest = _mapper.Map<RegistrationRequest>(registerVM);
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
