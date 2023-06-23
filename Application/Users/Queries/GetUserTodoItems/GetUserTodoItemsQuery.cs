using MediatR;
using TodoList.Application.Responses.User;

namespace TodoList.Application.Users.Queries.GetUserTodoItems;

public record GetUserTodoItemsQuery(Guid UserId) : IRequest<GetUserTodoItemsResponse?>;