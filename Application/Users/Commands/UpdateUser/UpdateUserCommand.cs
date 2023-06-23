using MediatR;
using TodoList.Application.Requests.User;

namespace TodoList.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest Request) : IRequest<UpdateUserResult>;