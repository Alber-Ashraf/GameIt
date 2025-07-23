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

    public string UserId { get => _contextAccessor.HttpContext?.User?.FindFirstValue("uid"); }

    public async Task<User> GetUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return new User
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.FirstName,
            Lastname = user.LastName
        };
    }

    public async Task<List<User>> GetUsers()
    {
        var employees = await _userManager.GetUsersInRoleAsync("User");

        return employees.Select(q => new User
        {
            Id = q.Id,
            Email = q.Email,
            Firstname = q.FirstName,
            Lastname = q.LastName
        }).ToList();
    }
}
