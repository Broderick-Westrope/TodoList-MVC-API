using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Repositories;

public interface IUserRepository : IRepository<UserAggregate>
{
    public Task<UserAggregate?> GetWithInclude(Guid id);
    public Task<IEnumerable<UserAggregate>?> GetAllWithInclude();
}