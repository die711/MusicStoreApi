using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class SaleRepository : RepositoryBase<Sale>, ISaleRepository
{
    public SaleRepository(MusicStoreDbContext context) : base(context)
    {
    }

    public async Task CreateTransaction()
    {
        await Context.Database.BeginTransactionAsync();
    }

    public async Task RollBackAsync()
    {
        await Context.Database.RollbackTransactionAsync();
    }

    public override async Task<long> AddAsync(Sale entity)
    {
        entity.SaleDate = DateTime.Now;
        var lastNumber = await Context.Set<Sale>().CountAsync() + 1;
        entity.OperationNumber = $"{lastNumber:00000}";

        await Context.AddAsync(entity);
        return entity.Id;
    }

    public override async Task<Sale?> FindByIdAsync(long id)
    {
        return await Context.Set<Sale>()
            .Include(p => p.Concert)
            .ThenInclude(p => p.Genre)
            .Include(p => p.Customer)
            .Where(p => p.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    
    
}