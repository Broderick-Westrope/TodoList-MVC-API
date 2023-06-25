using MediatR;

namespace TodoList.Application.TodoItems.Commands.AddTodoItem;

public record AddTodoItemCommand(CreateTodoItemRequest Request) : IRequest<AddTodoItemResult?>;