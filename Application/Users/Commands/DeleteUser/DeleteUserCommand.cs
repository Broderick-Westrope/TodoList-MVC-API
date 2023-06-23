using MediatR;

namespace TodoList.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<DeleteUserResult>;