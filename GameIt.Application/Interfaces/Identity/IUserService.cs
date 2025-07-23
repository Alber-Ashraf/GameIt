using GameIt.Application.Models.Identity;

namespace GameIt.Application.Interfaces.Identity;

public interface IUserService
{
    Task<List<User>> GetUsers();
    Task<User> GetUser(string userId);
}
