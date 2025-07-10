using GameIt.Application.Interfaces.Persistence;
using GameIt.Persistence.DatabaseContext;
using GameIt.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameIt.Persistence;
public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register the DbContext with the connection string from configuration
        services.AddDbContext<GameItDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("GameItDbConnectionString")));

        // Register repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
