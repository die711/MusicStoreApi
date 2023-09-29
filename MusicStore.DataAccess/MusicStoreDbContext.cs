using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.DataAccess;

public class MusicStoreDbContext : DbContext
{

    public MusicStoreDbContext(DbContextOptions<MusicStoreDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}