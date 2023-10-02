using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Entities.Info;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class ConcertRepository : RepositoryBase<Concert>, IConcertRepository
{
    public ConcertRepository(MusicStoreDbContext context) : base(context)
    {
        
        
    }


    public override async Task<Concert?> FindByIdAsync(long id)
    {
        return await Context.Set<Concert>()
            .Include(p => p.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<(ICollection<ConcertInfo> Collection, int Total)> ListAsync(string? filter, int page, int rows)
    {
        Expression<Func<Concert, bool>> predicate = p => p.Status && p.Title.Contains(filter ?? string.Empty);
        Expression<Func<Concert, ConcertInfo>> selector = p => new ConcertInfo
        {
            Id = p.Id,
            Genre = p.Genre.Name,
            Title = p.Title,
            Description = p.Description,
            Place = p.Place,
            UnitPrice = p.UnitPrice,
            DateEvent = p.DateEvent.ToString("yyyy-MM-dd"),
            TimeEvent = p.DateEvent.ToString("HH:mm"),
            TicketsQuantity = p.TicketsQuantity,
            ImageUrl = p.ImageUrl,
            Status = p.Status ? !p.Finalized ? "Activo" : "Finalizado" : "Inactivo"
        };

        Expression<Func<Concert, string>> orderBy = p => p.Title;

        return await ListAsync(predicate, selector, orderBy, page, rows);

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