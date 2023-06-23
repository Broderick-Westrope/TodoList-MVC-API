using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Projects.Commands.UpdateProject;
using TodoList.Application.Requests.TodoItem;
using TodoList.Domain;

namespace TodoList.Application.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemHandler : IRequestHandler<UpdateTodoItemCommand, UpdateTodoItemResult>
{
    private readonly IUserRepository _userRepository;

    public UpdateTodoItemHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateTodoItemResult> Handle(UpdateTodoItemCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var user = await _userRepository.GetByTodoItemId(command.TodoItemId, cancellationToken);
        if (user == null) return new UpdateTodoItemResult(false);

        var todoItem = user.TodoItems.First(x => x.Id == command.TodoItemId);
        todoItem.Title = request.Title;
        todoItem.Description = request.Description;
        todoItem.DueDate = request.DueDate;
        todoItem.IsCompleted = request.IsCompleted;

        _userRepository.Update(user);

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _userRepository.GetByProjectId(command.TodoItemId, cancellationToken) == null)
                return new UpdateTodoItemResult(false);
            throw;
        }

        return new UpdateTodoItemResult(true);
    }
}