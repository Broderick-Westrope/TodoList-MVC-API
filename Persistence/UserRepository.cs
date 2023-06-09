using Microsoft.EntityFrameworkCore;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace TodoList.Persistence;

public class UserRepository : IUserRepository
{
    private readonly TodoContext _context;

    public UserRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task<UserAggregate?> Get(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task Add(UserAggregate user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Remove(UserAggregate user)
    {
        _context.Users.Remove(user);
    }

    public void Update(UserAggregate user)
    {
        _context.Users
            .Update(user);
    }

    public async Task<UserAggregate?> GetByProjectId(Guid projectId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(
            x => x.Projects.FirstOrDefault(y => y.Id == projectId) != null, cancellationToken);
    }

    public async Task<UserAggregate?> GetByTodoItemId(Guid todoItemId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(
            x => x.TodoItems.FirstOrDefault(y => y.Id == todoItemId) != null, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}