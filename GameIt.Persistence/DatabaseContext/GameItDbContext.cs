using GameIt.Domain;
using GameIt.Domain.Common;
using GameIt.Domain.EntityMapping;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.DatabaseContext;

public class GameItDbContext : DbContext
{
    public GameItDbContext(DbContextOptions<GameItDbContext> options) : base(options)
    {

    }

    public DbSet<Game> Games { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> comments { get; set; }
    public DbSet<Discount> discounts { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameMapping());
        modelBuilder.ApplyConfiguration(new CategoryMapping());
        modelBuilder.ApplyConfiguration(new CommentMapping());
        modelBuilder.ApplyConfiguration(new DiscountMapping());
        modelBuilder.ApplyConfiguration(new PublisherMapping());
        modelBuilder.ApplyConfiguration(new PurchaseMapping());
        modelBuilder.ApplyConfiguration(new ReviewMapping());
        modelBuilder.ApplyConfiguration(new WishlistMapping());
        modelBuilder.ApplyConfiguration(new ApplicationUserMapping());

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set the CreatedAt and UpdatedAt properties
        foreach (var entry in ChangeTracker.Entries<BaseEntity>()
            .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
        {
            entry.Entity.UpdatedAt = DateTime.Now;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }


}
