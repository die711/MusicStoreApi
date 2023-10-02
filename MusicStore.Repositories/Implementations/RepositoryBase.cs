using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{

    public RepositoryBase(DbContext context)
    {
        
    }
    
    public Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TInfo>> selector, Expression<Func<TEntity, TKey>> orderBy, int page, int rows)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<TInfo>> ListAsync<TInfo>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TInfo>> selector)
    {
        throw new NotImplementedException();
    }

    public Task<long> AddAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> FindByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }
}