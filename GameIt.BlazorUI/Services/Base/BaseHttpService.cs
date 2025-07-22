namespace GameIt.BlazorUI.Services.Base;

public class BaseHttpService
{
    protected readonly IClient _client;
    public BaseHttpService(IClient client)
    {
        _client = client;
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
}
