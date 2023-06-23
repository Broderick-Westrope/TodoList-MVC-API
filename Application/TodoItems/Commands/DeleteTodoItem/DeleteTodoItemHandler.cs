using MediatR;
using TodoList.Domain;

namespace TodoList.Application.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemHandler : IRequestHandler<DeleteTodoItemCommand, DeleteTodoItemResult>
{
    private readonly IUserRepository _userRepository;

    public DeleteTodoItemHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<DeleteTodoItemResult> Handle(DeleteTodoItemCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByTodoItemId(command.TodoItemId, cancellationToken);
        if (user == null) return new DeleteTodoItemResult(false);

        user.DeleteTodoItem(command.TodoItemId);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new DeleteTodoItemResult(true);
    }
}