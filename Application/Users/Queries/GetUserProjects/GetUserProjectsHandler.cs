using Mapster;
using MediatR;
using TodoList.Application.Responses.Project;
using TodoList.Application.Responses.User;
using TodoList.Domain;

namespace TodoList.Application.Users.Queries.GetUserProjects;

public class GetUserProjectsQueryHandler : IRequestHandler<GetUserProjectsQuery, GetUserProjectsResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserProjectsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserProjectsResponse?> Handle(GetUserProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(request.UserId, cancellationToken);
        var projects = user?.Projects.Adapt<List<GetProjectResponse>>();
        return projects == null ? null : new GetUserProjectsResponse(projects);
    }
}