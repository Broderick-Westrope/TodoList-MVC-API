using Mapster;
using MapsterMapper;
using MediatR;
using TodoList.Application.Responses.User;
using TodoList.Domain;

namespace TodoList.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse?>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<GetUserResponse?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetWithInclude(request.UserId, cancellationToken);
        return result?.Adapt<GetUserResponse>();
    }
}