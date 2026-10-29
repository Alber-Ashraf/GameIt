using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GameIt.BlazorUI.Core.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    public const string TokenKey = "gameit_jwt_token";

    private readonly ILocalStorageService _localStorage;
    private readonly JwtSecurityTokenHandler _jwtHandler = new();
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var savedToken = await _localStorage.GetItemAsync<string>(TokenKey);
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return Anonymous;
            }

            var tokenContent = _jwtHandler.ReadJwtToken(savedToken);
            if (tokenContent.ValidTo < DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync(TokenKey);
                return Anonymous;
            }

            var claims = BuildClaims(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role));
            return new AuthenticationState(user);
        }
        catch
        {
            return Anonymous;
        }
    }

    public async Task LoggedIn(string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
        var tokenContent = _jwtHandler.ReadJwtToken(token);
        var claims = BuildClaims(tokenContent);
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role));
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task LoggedOut()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    public async Task<string?> GetUserIdAsync()
    {
        var state = await GetAuthenticationStateAsync();
        return state.User.FindFirst("uid")?.Value;
    }

    public async Task<bool> IsAdminAsync()
    {
        var state = await GetAuthenticationStateAsync();
        return state.User.IsInRole("Administrator");
    }

    private static List<Claim> BuildClaims(JwtSecurityToken tokenContent)
    {
        var claims = tokenContent.Claims.ToList();

        var sub = tokenContent.Subject;
        if (!string.IsNullOrWhiteSpace(sub) && !claims.Any(c => c.Type == ClaimTypes.Name))
        {
            claims.Add(new Claim(ClaimTypes.Name, sub));
        }

        // Normalize role claims regardless of schema URI or short "role" key
        var roleValues = claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .Distinct()
            .ToList();

        foreach (var role in roleValues)
        {
            if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        return claims;
    }
}
