using Mapster;
using MapsterMapper;
using MediatR;
using TodoList.Application.Responses.Project;
using TodoList.Application.Responses.User;
using TodoList.Application.Users.Queries.GetUser;
using TodoList.Domain;

namespace TodoList.Application.Users.Queries.GetUserProjects;

public class GetUserProjectsQueryHandler: IRequestHandler<GetUserProjectsQuery, GetUserProjectsResponse?>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetUserProjectsQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<GetUserProjectsResponse?> Handle(GetUserProjectsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(request.UserId, cancellationToken);
        var projects = user?.Projects.Adapt<List<GetProjectResponse>>();
        return projects == null ? null : new GetUserProjectsResponse(projects);
    }
}