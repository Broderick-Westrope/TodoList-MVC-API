using TodoList.Domain.Entities;

namespace TodoList.Domain;

public interface IUserRepository
{
    Task<UserAggregate?> Get(Guid userId, CancellationToken cancellationToken);
    public Task Add(UserAggregate user, CancellationToken cancellationToken);
    public void Remove(UserAggregate user);
    public void Update(UserAggregate user);
    public Task<UserAggregate?> GetByProjectId(Guid projectId, CancellationToken cancellationToken);
    public Task<UserAggregate?> GetByTodoItemId(Guid todoItemId, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}