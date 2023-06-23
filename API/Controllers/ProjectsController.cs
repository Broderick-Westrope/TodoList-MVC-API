using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Requests.Project;
using TodoList.Application.Responses.Project;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public ProjectsController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/Projects/:projectId
    [HttpGet("{projectId}")]
    public async Task<ActionResult<GetProjectResponse>> GetProject([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByProjectId(projectId, cancellationToken);
        if (user == null) return NotFound();

        var project = user.Projects.First(x => x.Id == projectId);
        var response = project.Adapt<GetProjectResponse>();

        return Ok(response);
    }

    // PUT: api/Projects/:projectId
    [HttpPut("{projectId}")]
    public async Task<IActionResult> PutProject([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByProjectId(projectId, cancellationToken);
        if (user == null) return NotFound();

        var project = user.Projects.First(x => x.Id == projectId);
        project.Title = request.Title;

        _userRepository.Update(user);

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _userRepository.GetByProjectId(projectId, cancellationToken) == null)
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Projects
    [HttpPost]
    public async Task<ActionResult<CreateProjectResponse>> PostProject([FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(request.UserId, cancellationToken);
        if (user == null) return BadRequest("Could not find user with the given User ID.");

        var projectId = Guid.NewGuid();
        var project = new Project(projectId, request.Title);
        user.AddProject(project);

        await _userRepository.SaveChangesAsync(cancellationToken);

        var response = project.Adapt<CreateProjectResponse>();
        return CreatedAtAction(nameof(GetProject), new { projectId }, response);
    }

    // DELETE: api/Projects/:projectId
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByProjectId(projectId, cancellationToken);
        if (user == null) return BadRequest("Could not find user with the given Project ID");

        user.DeleteProject(projectId);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}