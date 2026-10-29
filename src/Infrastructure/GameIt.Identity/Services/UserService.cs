using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Identity;
using GameIt.Application.Models.Identity;
using GameIt.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GameIt.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public string? UserId => _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

    public async Task<User> GetUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        return new User
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Firstname = user.FirstName,
            Lastname = user.LastName
        };
    }

    public async Task<List<User>> GetUsers()
    {
        var users = await _userManager.GetUsersInRoleAsync("User");

        return users.Select(q => new User
        {
            Id = q.Id,
            Email = q.Email ?? string.Empty,
            Firstname = q.FirstName,
            Lastname = q.LastName
        }).ToList();
    }
}
