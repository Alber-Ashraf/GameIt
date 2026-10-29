using Blazored.LocalStorage;
using GameIt.BlazorUI.Core.Auth;
using System.Net.Http.Headers;

namespace GameIt.BlazorUI.Core.Http;

public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public JwtAuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>(CustomAuthStateProvider.TokenKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch
        {
            // Ignore localStorage unavailability during prerender/init
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
