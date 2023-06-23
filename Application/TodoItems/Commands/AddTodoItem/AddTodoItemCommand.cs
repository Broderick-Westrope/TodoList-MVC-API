using MediatR;
using TodoList.Application.Requests.Project;
using TodoList.Application.Requests.TodoItem;

namespace TodoList.Application.TodoItems.Commands.AddTodoItem;

public record AddTodoItemCommand(CreateTodoItemRequest Request) : IRequest<AddTodoItemResult?>;