using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Repositories;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetWithInclude(Guid id);
    public Task<IEnumerable<User>?> GetAllWithInclude();
}