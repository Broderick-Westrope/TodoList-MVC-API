using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.DbModels;
using TodoList.MVC.API.Repositories;
using TodoList.MVC.API.Requests.TodoItem;
using TodoList.MVC.API.Responses.TodoItem;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public TodoItemsController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/TodoItems/:todoItemId
    [HttpGet("{todoItemId}")]
    public async Task<ActionResult<GetTodoItemResponse>> GetTodoItem(Guid todoItemId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByTodoItemId(todoItemId, cancellationToken);
        if (user == null) return NotFound();

        var todoItem = user.TodoItems.First(x => x.Id == todoItemId);
        var response = todoItem.Adapt<GetTodoItemResponse>();

        return Ok(response);
    }

    // PUT: api/TodoItems/:todoItemId
    [HttpPut("{todoItemId}")]
    public async Task<IActionResult> PutTodoItem([FromRoute] Guid todoItemId, [FromBody] UpdateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByTodoItemId(todoItemId, cancellationToken);
        if (user == null) return NotFound();

        var todoItem = user.TodoItems.First(x => x.Id == todoItemId);
        todoItem.Title = request.Title;
        todoItem.Description = request.Description;
        todoItem.DueDate = request.DueDate;
        todoItem.IsCompleted = request.IsCompleted;

        _userRepository.Update(user);

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _userRepository.GetByTodoItemId(todoItemId, cancellationToken) == null)
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/TodoItems
    [HttpPost]
    public async Task<ActionResult<CreateTodoItemResponse>> PostTodoItem([FromBody] CreateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(request.UserId, cancellationToken);
        if (user == null) return BadRequest("Could not find user with the given User ID.");

        var todoItemId = Guid.NewGuid();
        var todoItem = new TodoItem(todoItemId, request.Title, request.Description, request.DueDate);
        user.AddTodoItem(todoItem);

        await _userRepository.SaveChangesAsync(cancellationToken);

        var response = todoItem.Adapt<CreateTodoItemResponse>();
        return CreatedAtAction(nameof(GetTodoItem), new { todoItemId }, response);
    }


    // DELETE: api/TodoItems/:todoItemId
    [HttpDelete("{todoItemId}")]
    public async Task<IActionResult> DeleteTodoItem([FromRoute] Guid todoItemId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByTodoItemId(todoItemId, cancellationToken);
        if (user == null) return BadRequest("Could not find user with the given Project ID");

        user.DeleteTodoItem(todoItemId);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}