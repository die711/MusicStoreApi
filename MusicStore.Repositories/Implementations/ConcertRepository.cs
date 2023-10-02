using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class ConcertRepository : RepositoryBase<Concert>, IConcertRepository
{
    public ConcertRepository(DbContext context) : base(context)
    {
    }


    public async Task<Concert?> FindByIdAsync(long id)
    {
        return await Context.Set<Concert>()
            .Include(p => p.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task FinalizeAsync(long id)
    {
        var entity = await FindByIdAsync(id);

        if (entity is not null)
        {
            entity.Finalized = true;
            await UpdateAsync();
        }
        else
            throw new InvalidOperationException($"Unable to finalize entity {id}");
    }
}