using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Entities.Info;
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

    public override async Task UpdateAsync()
    {
        //await Context.Database.CommitTransactionAsync();
        await Context.SaveChangesAsync();
    }

    public async Task RollBackAsync()
    {
        await Context.Database.RollbackTransactionAsync();
    }

    public async Task<IEnumerable<ReportInfo>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd)
    {
        var query = Context.Set<ReportInfo>().FromSqlRaw("Exec uspReportSales {0}, {1}", dateStart, dateEnd);
        return await query.ToListAsync();
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

    public override async Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo, TKey>(Expression<Func<Sale, bool>> predicate, Expression<Func<Sale, TInfo>> selector, Expression<Func<Sale, TKey>> orderBy, int page, int rows)
    {
        var collection = await Context.Set<Sale>()
            .Include(p => p.Concert)
            .ThenInclude(p => p.Genre)
            .Include(p => p.Customer)
            .Where(predicate)
            .OrderBy(orderBy)
            .Skip((page - 1) * rows)
            .Take(rows)
            .AsNoTracking()
            .Select(selector)
            .ToListAsync();

        var total = await Context.Set<Sale>()
            .Where(predicate)
            .CountAsync();

        return (collection, total);
    }
}