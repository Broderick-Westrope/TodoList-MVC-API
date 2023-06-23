using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace TodoList.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _userRepository.Update(new UserAggregate(command.UserId, request.Email, request.Password));

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _userRepository.Get(command.UserId, cancellationToken) == null)
                return new UpdateUserResult(false);
            throw;
        }

        return new UpdateUserResult(true);
    }
}