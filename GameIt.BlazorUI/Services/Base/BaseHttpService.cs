using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace GameIt.BlazorUI.Services.Base;

public class BaseHttpService
{
    protected readonly IClient _client;
    protected readonly ILocalStorageService _localStorage;
    public BaseHttpService(IClient client, ILocalStorageService localStorage)
    {
        _client = client;
        _localStorage = localStorage;
    }

    protected Response<Guid> ConvertApiExceptions<Guid>(ApiException apiException)
    {
        if (apiException.StatusCode == 400)
        {
            return new Response<Guid>
            {
                Message = "Invalid Data was submitted.",
                ValidationErrors = apiException.Response,
                Success = false,
            };
        }
        else if (apiException.StatusCode == 404)
        {
            return new Response<Guid>
            {
                Message = "The requested resource was not found.",
                ValidationErrors = apiException.Response,
                Success = false,
            };
        }
        else if (apiException.StatusCode == 500)
        {
            return new Response<Guid>
            {
                Message = "An internal server error occurred.",
                ValidationErrors = apiException.Response,
                Success = false,
            };
        }
        else
        {
            return new Response<Guid>
            {
                Message = "An unexpected error occurred.",
                ValidationErrors = apiException.Response,
                Success = false,
            };
        }
    }

    protected async Task AddBearerToken()
    {
        if (await _localStorage.ContainKeyAsync("token"))
            _client.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await _localStorage.GetItemAsync<string>("token"));
    }
}
