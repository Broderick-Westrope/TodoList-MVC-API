using MediatR;
using TodoList.Application.Projects.Commands.AddProject;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace TodoList.Application.TodoItems.Commands.AddTodoItem;

public class AddTodoItemHandler : IRequestHandler<AddTodoItemCommand, AddTodoItemResult?>
{
    private readonly IUserRepository _userRepository;

    public AddTodoItemHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AddTodoItemResult?> Handle(AddTodoItemCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userRepository.Get(request.UserId, cancellationToken);
        if (user == null) return null;

        var todoItemId = Guid.NewGuid();
        var todoItem = new TodoItem(todoItemId, request.Title, request.Description, request.DueDate);

        user?.AddTodoItem(todoItem);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AddTodoItemResult(todoItemId);
    }
}