using MusicStore.Entities;

namespace MusicStore.Repositories.Interfaces;

public interface ISaleRepository : IRepositoryBase<Sale>
{
    Task CreateTransaction();
    Task RollBackAsync();
}