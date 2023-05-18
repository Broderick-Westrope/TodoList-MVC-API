using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemController : ControllerBase
{
    private readonly TodoDbContext _dbContext;

    public TodoItemController(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: api/TodoItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        return await _dbContext.TodoItems.ToListAsync();
    }

    // GET: api/TodoItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(Guid id)
    {
        var todoItem = await _dbContext.TodoItems.FindAsync(id);

        if (todoItem == null) return NotFound();

        return todoItem;
    }

    // PUT: api/TodoItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
    {
        if (id != todoItem.Id) return BadRequest();

        _dbContext.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoItemExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/TodoItem
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem([FromBody] CreateTodoItemRequest request)
    {
        var todoItem = new TodoItem(request.UserId, request.Title, request.Description, request.DueDate);

        _dbContext.TodoItems.Add(todoItem);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }

    // DELETE: api/TodoItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(Guid id)
    {
        var todoItem = await _dbContext.TodoItems.FindAsync(id);
        if (todoItem == null) return NotFound();

        _dbContext.TodoItems.Remove(todoItem);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(Guid id)
    {
        return (_dbContext.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}