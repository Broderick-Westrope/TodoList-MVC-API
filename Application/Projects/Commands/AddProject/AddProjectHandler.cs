using MediatR;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace TodoList.Application.Projects.Commands.AddProject;

public class AddProjectHandler : IRequestHandler<AddProjectCommand, AddProjectResult?>
{
    private readonly IUserRepository _userRepository;

    public AddProjectHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AddProjectResult?> Handle(AddProjectCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await _userRepository.Get(request.UserId, cancellationToken);
        if (user == null) return null;

        var projectId = Guid.NewGuid();
        var project = new Project(projectId, request.Title);

        user?.AddProject(project);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AddProjectResult(projectId);
    }
}