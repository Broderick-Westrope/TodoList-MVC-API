using MediatR;

namespace TodoList.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand(Guid TodoItemId, UpdateTodoItemRequest Request) : IRequest<UpdateTodoItemResult>;