using GameIt.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Identity.DbContext;

internal class GameItIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public GameItIdentityDbContext(DbContextOptions<GameItIdentityDbContext> options)
        : base(options) {}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(GameItIdentityDbContext).Assembly);
    }
}

