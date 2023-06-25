using MediatR;

namespace TodoList.Application.TodoItems.Queries.GetTodoItem;

public record GetTodoItemQuery(Guid TodoItemId) : IRequest<GetTodoItemResponse?>;