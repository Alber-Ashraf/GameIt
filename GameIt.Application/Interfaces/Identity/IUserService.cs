using GameIt.Application.Models.Identity;

namespace GameIt.Application.Interfaces.Identity;

public interface IUserService
{
    Task<List<Employee>> GetEmployees();
    Task<Employee> GetEmployee(string userId);
}
