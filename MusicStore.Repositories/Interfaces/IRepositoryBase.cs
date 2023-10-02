using System.Collections;
using System.Linq.Expressions;
using MusicStore.Entities;

namespace MusicStore.Repositories.Interfaces;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    //lista de objectos basados en el entity
    Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate);
    
    //Lista de objetos transformados en un objeto Info que tiene paginacion y ademas un selector
    Task<(ICollection<TInfo> Collection, int Total)> ListAsync<TInfo,TKey>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TInfo>> selector,
        Expression<Func<TEntity, TKey>> orderBy,
        int page, int rows);
    
    //Lista de objectos con un selector con filtro
    Task<ICollection<TInfo>> ListAsync<TInfo>(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TInfo>> selector);
    
    Task<long> AddAsync(TEntity entity);
    
    Task<TEntity?> FindByIdAsync(long id);

    Task UpdateAsync();
    
    Task DeleteAsync(long id);
}