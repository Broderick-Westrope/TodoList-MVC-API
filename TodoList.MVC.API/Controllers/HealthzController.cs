using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthzController : ControllerBase
{
    private readonly TodoContext _todoContext;

    public HealthzController(TodoContext todoContext)
    {
        _todoContext = todoContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var results = new List<string>
        {
            "Request Received.",
            "Attempting to connect to database."
        };

        try
        {
            await _todoContext.Database.ExecuteSqlAsync($"SELECT 1", cancellationToken);
            results.Add("Successfully connected to database.");
        }
        catch (Exception e)
        {
            results.Add("Failed to connect to database.");
            results.Add(e.ToString());
            Console.WriteLine(e);
        }

        return Ok(new { results });
    }
}