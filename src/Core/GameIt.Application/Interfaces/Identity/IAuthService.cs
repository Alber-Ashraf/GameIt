using GameIt.Application.Models.Identity;

namespace GameIt.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<AuthResponse> Login(AuthRequest authRequest);
    Task<RegistrationResponse> Register(RegistrationRequest registrationRequest);
}
