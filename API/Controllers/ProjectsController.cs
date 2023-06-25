using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Projects.Commands.AddProject;
using TodoList.Application.Projects.Commands.DeleteProject;
using TodoList.Application.Projects.Commands.UpdateProject;
using TodoList.Application.Projects.Queries.GetProject;
using TodoList.Application.Requests.Project;
using TodoList.Application.Responses;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ApiControllerBase
{
    // GET: api/Projects/:projectId
    [HttpGet("{projectId}")]
    public async Task<ActionResult<GetProjectResponse>> GetProject([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetProjectQuery(projectId), cancellationToken);

        return response == null ? NotFound() : Ok(response);
    }

    // PUT: api/Projects/:projectId
    [HttpPut("{projectId}")]
    public async Task<IActionResult> PutProject([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new UpdateProjectCommand(projectId, request), cancellationToken);

        return result.WasProjectFound ? NoContent() : NotFound();
    }

    // POST: api/Projects
    [HttpPost]
    public async Task<ActionResult<GetProjectResponse>> PostProject([FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new AddProjectCommand(request), cancellationToken);
        if (result == null) return BadRequest("Could not find user with the given User ID.");

        var response = new GetProjectResponse(result.ProjectId, request.Title);
        return CreatedAtAction(nameof(GetProject), new { projectId = result.ProjectId }, response);
    }

    // DELETE: api/Projects/:projectId
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteProjectCommand(projectId), cancellationToken);
        if (!result.WasUserFound) return BadRequest("Could not find user with the given Project ID");

        return NoContent();
    }
}