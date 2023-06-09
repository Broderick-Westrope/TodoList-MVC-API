using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Repositories;

public interface IUserRepository : IRepository<UserAggregateRoot>
{
    public Task<UserAggregateRoot?> GetWithInclude(Guid id);
    public Task<IEnumerable<UserAggregateRoot>?> GetAllWithInclude();
}