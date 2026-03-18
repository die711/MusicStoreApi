using MusicStore.DataAccess;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly MusicStoreDbContext _context;

    public UnitOfWork(MusicStoreDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
