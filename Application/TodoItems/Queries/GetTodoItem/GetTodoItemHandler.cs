using Mapster;
using MediatR;
using TodoList.Application.Projects.Queries.GetProject;
using TodoList.Application.Responses;
using TodoList.Application.Responses.TodoItem;
using TodoList.Domain;

namespace TodoList.Application.TodoItems.Queries.GetTodoItem;

public class GetTodoItemHandler : IRequestHandler<GetTodoItemQuery, GetTodoItemResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetTodoItemHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetTodoItemResponse?> Handle(GetTodoItemQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByTodoItemId(query.TodoItemId, cancellationToken);
        var todoItem = user?.TodoItems.First(x => x.Id == query.TodoItemId);
        return todoItem?.Adapt<GetTodoItemResponse>();
    }
}