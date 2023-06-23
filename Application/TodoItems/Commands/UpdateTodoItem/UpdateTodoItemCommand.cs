using MediatR;
using TodoList.Application.Requests.Project;
using TodoList.Application.Requests.TodoItem;

namespace TodoList.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand(Guid TodoItemId, UpdateTodoItemRequest Request) : IRequest<UpdateTodoItemResult>;