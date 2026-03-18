using System.Threading;
using System.Threading.Tasks;

namespace MusicStore.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
