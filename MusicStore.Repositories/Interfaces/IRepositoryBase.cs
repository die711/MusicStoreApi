using System.Collections;
using System.Linq.Expressions;
using MusicStore.Entities;

namespace MusicStore.Repositories.Interfaces;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo,TKey>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TInfo>> selector,
        Expression<Func<TEntity, TKey>> orderBy,
        int page, int rows);
    
    Task<ICollection<TInfo>> ListAsync<TInfo>(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TInfo>> selector);
    
    Task<long> AddAsync(TEntity entity);
    
    Task<TEntity?> FindByIdAsync(long id);
    
    Task DeleteAsync(long id);
}