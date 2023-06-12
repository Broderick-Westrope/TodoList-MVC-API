using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests.Project;
using TodoList.MVC.API.Responses.Project;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly TodoContext _todoContext;

    public ProjectController(TodoContext todoContext)
    {
        _todoContext = todoContext;
    }

    // GET: api/Project
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var projectList = await _todoContext
            .Projects
            .ToListAsync();

        var projects = from p in projectList
            select new GetProjectResponse(p.Id, p.Title);

        return Ok(new GetAllProjectsResponse(projects.ToList()));
    }

    // GET: api/Project/5
    [HttpGet("{id}")]
    //! FIXED
    public async Task<ActionResult<GetProjectResponse>> GetProject([FromRoute] Guid id)
    {
        var user = await _todoContext
            .Users.FirstOrDefaultAsync(x => x.Projects.FirstOrDefault(x => x.Id == id) != null);

        if (user == null) return NotFound();
        var project = user.Projects.First(x => x.Id == id);

        var response = new GetProjectResponse(project.Id, project.Title);

        return Ok(response);
    }

    // PUT: api/Project/5
    [HttpPut("{id}")]
    //! FIXED
    public async Task<IActionResult> PutProject([FromRoute] Guid id, [FromBody] UpdateProjectRequest request)
    {
        var user = await _todoContext.Users.FirstAsync(x => x.Projects.FirstOrDefault(x => x.Id == id) != null);

        var project = user.Projects.First(x => x.Id == id);
        project.Title = request.Title;
        
        _todoContext
            .Entry(user)
            .State = EntityState.Modified;

        try
        {
            await _todoContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Project
    [HttpPost]
    public async Task<ActionResult<CreateProjectResponse>> PostProject([FromBody] CreateProjectRequest request)
    {
        var projectId = Guid.NewGuid();

        _todoContext
            .Projects
            .Add(new Project(projectId, request.Title));
        await _todoContext
            .SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = projectId },
            new CreateProjectResponse(projectId, request.Title));
    }

    // DELETE: api/Project/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
    {
        var project = await _todoContext.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _todoContext.Projects.Remove(project);
        await _todoContext.SaveChangesAsync();

        return NoContent();
    }

    private bool ProjectExists(Guid id)
    {
        return (_todoContext.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}