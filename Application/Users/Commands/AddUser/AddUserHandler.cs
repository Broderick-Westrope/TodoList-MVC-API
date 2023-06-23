using MapsterMapper;
using MediatR;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace TodoList.Application.Users.Commands.AddUser;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, AddUserResult>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AddUserResult> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var request = command.Request;

        var user = new UserAggregate(userId, request.Email, request.Password);
        await _userRepository.Add(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AddUserResult(userId);
    }
}