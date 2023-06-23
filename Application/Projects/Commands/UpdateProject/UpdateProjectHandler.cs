using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain;

namespace TodoList.Application.Projects.Commands.UpdateProject;

public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, UpdateProjectResult>
{
    private readonly IUserRepository _userRepository;

    public UpdateProjectHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateProjectResult> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByProjectId(command.ProjectId, cancellationToken);
        if (user == null) return new UpdateProjectResult(false);

        var project = user.Projects.First(x => x.Id == command.ProjectId);
        project.Title = command.Request.Title;

        _userRepository.Update(user);

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _userRepository.GetByProjectId(command.ProjectId, cancellationToken) == null)
                return new UpdateProjectResult(false);
            throw;
        }

        return new UpdateProjectResult(true);
    }
}