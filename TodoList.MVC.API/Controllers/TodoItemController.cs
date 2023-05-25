using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests.TodoItem;
using TodoList.MVC.API.Responses.TodoItem;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemController : ControllerBase
{
    private readonly TodoContext _todoContext;

    public TodoItemController(TodoContext todoContext)
    {
        _todoContext = todoContext;
    }

    // GET: api/TodoItem
    [HttpGet]
    public async Task<ActionResult<GetAllTodoItemsResponse>> GetTodoItems()
    {
        var todoItemList = await _todoContext
            .TodoItems
            .ToListAsync();

        var todoItems = from t in todoItemList
            select new GetTodoItemResponse(t.Id, t.Title, t.Description, t.DueDate, t.IsCompleted, t.UserId);

        return Ok(new GetAllTodoItemsResponse(todoItems.ToList()));
    }

    // GET: api/TodoItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetTodoItemResponse>> GetTodoItem(Guid id)
    {
        var todoItem = await _todoContext
            .TodoItems
            .FindAsync(id);

        if (todoItem == null) return NotFound();

        var response = new GetTodoItemResponse(todoItem.Id, todoItem.Title, todoItem.Description, todoItem.DueDate,
            todoItem.IsCompleted, todoItem.UserId);

        return Ok(response);
    }

    // PUT: api/TodoItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem([FromRoute] Guid id, [FromBody] UpdateTodoItemRequest request)
    {
        _todoContext
            .Entry(new TodoItem(id, request.UserId, request.Title, request.Description, request.DueDate,
                request.IsCompleted))
            .State = EntityState.Modified;

        try
        {
            await _todoContext.SaveChangesAsync();
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
    public async Task<ActionResult<CreateTodoItemResponse>> PostTodoItem([FromBody] CreateTodoItemRequest request)
    {
        var todoItemId = Guid.NewGuid();

        _todoContext
            .TodoItems
            .Add(new TodoItem(todoItemId, request.UserId, request.Title, request.Description, request.DueDate));
        await _todoContext
            .SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItemId },
            new CreateTodoItemResponse(todoItemId, request.Title, request.Description, request.DueDate, false,
                request.UserId));
    }

    // DELETE: api/TodoItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem([FromRoute] Guid id)
    {
        var todoItem = await _todoContext.TodoItems.FindAsync(id);
        if (todoItem == null) return NotFound();

        _todoContext.TodoItems.Remove(todoItem);
        await _todoContext.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(Guid id)
    {
        return (_todoContext.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}