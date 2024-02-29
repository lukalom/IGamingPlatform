using IGamingPlatform.Domain;
using IGamingPlatform.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IGamingPlatform.Infrastructure.Persistence;

public class GamingPlatformDBContext : DbContext
{
    public GamingPlatformDBContext(DbContextOptions<GamingPlatformDBContext> options) : base(options)
    {
 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new BetConfiguration());
    }

    public DbSet<User> Users { get; init; }
    public DbSet<Bet> Bets { get; init; }
   
}