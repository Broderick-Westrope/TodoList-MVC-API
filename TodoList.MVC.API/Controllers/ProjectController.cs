using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly TodoDbContext _dbContext;

    public ProjectController(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: api/Project
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        return await _dbContext.Projects.ToListAsync();
    }

    // GET: api/Project/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(Guid id)
    {
        var project = await _dbContext.Projects.FindAsync(id);

        if (project == null) return NotFound();

        return project;
    }

    // PUT: api/Project/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(Guid id, Project project)
    {
        if (id != project.Id) return BadRequest();

        _dbContext.Entry(project).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
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
    public async Task<ActionResult<Project>> PostProject(CreateProjectRequest request)
    {
        var project = new Project(request.UserId, request.Title);

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction("GetProject", new { id = project.Id }, project);
    }

    // DELETE: api/Project/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _dbContext.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool ProjectExists(Guid id)
    {
        return (_dbContext.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}