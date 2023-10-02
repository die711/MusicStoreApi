using MusicStore.Entities;

namespace MusicStore.Repositories.Interfaces;

public interface IConcertRepository : IRepositoryBase<Concert>
{
    Task FinalizeAsync(long id);
}