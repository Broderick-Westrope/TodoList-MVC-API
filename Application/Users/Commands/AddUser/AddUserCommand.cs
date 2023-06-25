using MediatR;

namespace TodoList.Application.Users.Commands.AddUser;

public record AddUserCommand(CreateUserRequest Request) : IRequest<AddUserResult>;