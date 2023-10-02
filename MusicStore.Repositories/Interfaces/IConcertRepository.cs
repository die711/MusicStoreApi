using MusicStore.Entities;
using MusicStore.Entities.Info;

namespace MusicStore.Repositories.Interfaces;

public interface IConcertRepository : IRepositoryBase<Concert>
{

    Task<(ICollection<ConcertInfo> Collection, int Total)> ListAsync(string? filter, int page, int rows);
    Task FinalizeAsync(long id);
}