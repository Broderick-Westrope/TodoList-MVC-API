using MediatR;
using TodoList.Application.Requests.User;

namespace TodoList.Application.Users.Commands.AddUser;

public record AddUserCommand(CreateUserRequest Request) : IRequest<AddUserResult>;