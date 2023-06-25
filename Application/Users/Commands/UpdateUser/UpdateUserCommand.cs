using MediatR;

namespace TodoList.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest Request) : IRequest<UpdateUserResult>;