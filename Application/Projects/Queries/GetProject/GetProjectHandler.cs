using Mapster;
using MediatR;
using TodoList.Domain;

namespace TodoList.Application.Projects.Queries.GetProject;

public class GetProjectHandler : IRequestHandler<GetProjectQuery, GetProjectResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetProjectHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetProjectResponse?> Handle(GetProjectQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByProjectId(query.ProjectId, cancellationToken);
        var project = user?.Projects.First(x => x.Id == query.ProjectId);
        return project?.Adapt<GetProjectResponse>();
    }
}