using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests;
using TodoList.MVC.API.Responses;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoItemController(TodoContext context)
    {
        _context = context;
    }

    // GET: api/TodoItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        return await _context.TodoItems.ToListAsync();
    }

    // GET: api/TodoItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(Guid id)
    {
        var todoItem =  await  _context.TodoItems.FindAsync(id);

        if (todoItem == null) return NotFound();

        return todoItem;
    }

    // PUT: api/TodoItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
    {
        if (id != todoItem.Id) return BadRequest();

        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
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
        
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }

    // DELETE: api/TodoItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(Guid id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null) return NotFound();

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(Guid id)
    {
        return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}