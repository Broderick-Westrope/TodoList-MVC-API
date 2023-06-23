using MediatR;
using TodoList.Application.Responses;
using TodoList.Application.Responses.TodoItem;

namespace TodoList.Application.TodoItems.Queries.GetTodoItem;

public record GetTodoItemQuery(Guid TodoItemId) : IRequest<GetTodoItemResponse?>;