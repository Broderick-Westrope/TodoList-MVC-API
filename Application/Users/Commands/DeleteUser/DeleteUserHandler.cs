using MediatR;
using TodoList.Domain;

namespace TodoList.Application.Users.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<DeleteUserResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(command.UserId, cancellationToken);
        if (user == null) return new DeleteUserResult(false);

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new DeleteUserResult(true);
    }
}