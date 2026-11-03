using GameIt.API.Middlewares;
using GameIt.Application;
using GameIt.Identity;
using GameIt.Identity.DbContext;
using GameIt.Infrastructure;
using GameIt.Persistence;
using GameIt.Persistence.DatabaseContext;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Threading.RateLimiting;

namespace GameIt.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices();
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddIdentityServices(builder.Configuration);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();

        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "https://localhost:7294", "http://localhost:5036", "https://localhost:7142", "http://localhost:5173" };

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("BlazorClientPolicy",
                policy => policy.WithOrigins(allowedOrigins)
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("AuthPolicy", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 10;
                opt.QueueLimit = 0;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                if (builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true))
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GameItDbContext>();
                    dbContext.Database.Migrate();

                    var identityDbContext = scope.ServiceProvider.GetRequiredService<GameItIdentityDbContext>();
                    identityDbContext.Database.Migrate();

                    if (builder.Configuration.GetValue("Database:SeedDataOnStartup", true))
                    {
                        await GameItDatabaseSeeder.SeedAsync(dbContext);
                    }
                }

                RecurringJobsInitializer.Initialize(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Startup database migration, seeding, or Hangfire job initialization failed. Verify ConnectionStrings:GameItDbConnectionString.");
            }
        }

        if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("Swagger:Enabled", false))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseCors("BlazorClientPolicy");

        app.UseRateLimiter();

        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/health", () => Results.Ok(new
        {
            status = "Healthy",
            service = "GameIt.API",
            timestamp = DateTime.UtcNow
        })).AllowAnonymous();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        await app.RunAsync();
    }
}
