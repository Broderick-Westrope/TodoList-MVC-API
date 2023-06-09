using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly TodoContext _context;

    public UserRepository(TodoContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetWithInclude(Guid id)
    {
        return await _context
            .Users
            .Include(u => u.TodoItems)
            .Include(u => u.Projects)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>?> GetAllWithInclude()
    {
        return await _context
            .Users
            .Include(u => u.TodoItems)
            .Include(u => u.Projects)
            .ToListAsync();
    }
}