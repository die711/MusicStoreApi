using MusicStore.Entities;
using MusicStore.Entities.Info;

namespace MusicStore.Repositories.Interfaces;

public interface ISaleRepository : IRepositoryBase<Sale>
{
    Task CreateTransaction();
    Task RollBackAsync();
    Task<IEnumerable<ReportInfo>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd);

}