using MediatR;

namespace TodoList.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(Guid TodoItemId) : IRequest<DeleteTodoItemResult>;