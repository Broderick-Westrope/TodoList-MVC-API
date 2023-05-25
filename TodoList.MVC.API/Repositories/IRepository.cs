using System.Linq.Expressions;

namespace TodoList.MVC.API.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> Get(int id);
    Task<IEnumerable<TEntity>> GetAll();
    Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);

    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}