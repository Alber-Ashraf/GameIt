using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Identity;
using GameIt.Application.Models.Identity;
using GameIt.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameIt.Identity.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(
        IOptions<JwtSettings> jwtSettings,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<AuthResponse> Login(AuthRequest authRequest)
    {
        var user = await _userManager.FindByEmailAsync(authRequest.Email);
        if (user == null)
        {
            throw new NotFoundException($"User with {authRequest.Email} not found.", authRequest.Email);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, authRequest.Password, false);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"Credentials for '{authRequest.Email}' aren't valid.");
        }

        var jwtSecurityToken = await GenerateToken(user);

        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            Id = user.Id,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty
        };
    }

    public async Task<RegistrationResponse> Register(RegistrationRequest registrationRequest)
    {
        var user = new ApplicationUser
        {
            UserName = registrationRequest.UserName,
            Email = registrationRequest.Email,
            FirstName = registrationRequest.FirstName,
            LastName = registrationRequest.LastName,
            ProfilePictureUrl = registrationRequest.ProfilePictureUrl,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registrationRequest.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            return new RegistrationResponse { UserId = user.Id };
        }

        var str = new StringBuilder();
        foreach (var err in result.Errors)
        {
            str.AppendFormat("• {0}\n", err.Description);
        }

        throw new BadRequestException(str.ToString().TrimEnd());
    }

    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim("uid", user.Id)
        }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
           issuer: _jwtSettings.Issuer,
           audience: _jwtSettings.Audience,
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
           signingCredentials: signingCredentials);
    }
}
