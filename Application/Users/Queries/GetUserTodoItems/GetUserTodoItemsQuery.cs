using MediatR;

namespace TodoList.Application.Users.Queries.GetUserTodoItems;

public record GetUserTodoItemsQuery(Guid UserId) : IRequest<GetUserTodoItemsResponse?>;