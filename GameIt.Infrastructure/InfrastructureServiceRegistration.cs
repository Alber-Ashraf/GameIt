using GameIt.Application.Interfaces.Email;
using GameIt.Application.Interfaces.IDiscount;
using GameIt.Application.Interfaces.Logging;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Email;
using GameIt.Application.Models.Stripe;
using GameIt.Infrastructure.DiscountService;
using GameIt.Infrastructure.EmailService;
using GameIt.Infrastructure.Logging;
using GameIt.Infrastructure.Stripe;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameIt.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddTransient<IEmailSender, EmailSender>();

        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        services.AddHangfire(config =>
        config.UseSqlServerStorage(configuration.GetConnectionString("GameItDbConnectionString")));

        services.AddHangfireServer();

        services.AddScoped<IDiscountService, DiscountUpdate>();

        services.AddScoped<IStripeService, StripeService>();
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

        return services;
    }
}
    
