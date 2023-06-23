using Mapster;
using MediatR;
using TodoList.Application.Responses.User;
using TodoList.Domain;

namespace TodoList.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserResponse?> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var result = await _userRepository.Get(query.UserId, cancellationToken);
        return result?.Adapt<GetUserResponse>();
    }
}