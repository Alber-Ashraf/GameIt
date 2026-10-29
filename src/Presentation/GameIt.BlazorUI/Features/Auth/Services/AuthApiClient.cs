using GameIt.BlazorUI.Core.Auth;
using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Auth;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Auth.Services;

public interface IAuthApiClient
{
    Task<ApiResult<AuthResponseDto>> LoginAsync(AuthRequestDto request, CancellationToken ct = default);
    Task<ApiResult<RegistrationResponseDto>> RegisterAsync(RegistrationRequestDto request, CancellationToken ct = default);
    Task LogoutAsync();
}

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthApiClient(HttpClient http, CustomAuthStateProvider authStateProvider)
    {
        _http = http;
        _authStateProvider = authStateProvider;
    }

    public async Task<ApiResult<AuthResponseDto>> LoginAsync(AuthRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.AuthLogin, request, ct);
        var result = await ProblemDetailsParser.ToApiResultAsync<AuthResponseDto>(response);

        if (result.IsSuccess && result.Data is not null && !string.IsNullOrWhiteSpace(result.Data.Token))
        {
            await _authStateProvider.LoggedIn(result.Data.Token);
        }

        return result;
    }

    public async Task<ApiResult<RegistrationResponseDto>> RegisterAsync(RegistrationRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.AuthRegister, request, ct);
        return await ProblemDetailsParser.ToApiResultAsync<RegistrationResponseDto>(response);
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.LoggedOut();
    }
}
