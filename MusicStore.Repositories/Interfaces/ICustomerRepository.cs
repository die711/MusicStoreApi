using MusicStore.Entities;

namespace MusicStore.Repositories.Interfaces;

public interface ICustomerRepository : IRepositoryBase<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
}