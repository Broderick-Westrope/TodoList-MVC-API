using Mapster;
using MediatR;
using TodoList.Application.TodoItems.Queries.GetTodoItem;
using TodoList.Domain;

namespace TodoList.Application.Users.Queries.GetUserTodoItems;

public class GetUserTodoItemsQueryHandler : IRequestHandler<GetUserTodoItemsQuery, GetUserTodoItemsResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserTodoItemsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserTodoItemsResponse?> Handle(GetUserTodoItemsQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(query.UserId, cancellationToken);
        var todoItems = user?.TodoItems.Adapt<List<GetTodoItemResponse>>();
        return todoItems == null ? null : new GetUserTodoItemsResponse(todoItems);
    }
}