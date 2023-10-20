using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities.Info;

namespace MusicStore.DataAccess;

public class MusicStoreDbContext : IdentityDbContext<MusicStoreUserIdentity>
{

    public MusicStoreDbContext(DbContextOptions<MusicStoreDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<ReportInfo>()
            .HasNoKey();

        modelBuilder.Entity<ReportInfo>()
            .Property(p => p.Total)
            .HasPrecision(11, 2);

    }
}