using MediatR;
using TodoList.Domain;

namespace TodoList.Application.Projects.Commands.DeleteProject;

public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, DeleteProjectResult>
{
    private readonly IUserRepository _userRepository;

    public DeleteProjectHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<DeleteProjectResult> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByProjectId(command.ProjectId, cancellationToken);
        if (user == null) return new DeleteProjectResult(false);

        user.DeleteProject(command.ProjectId);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new DeleteProjectResult(true);
    }
}