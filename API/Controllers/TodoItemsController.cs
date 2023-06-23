using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Requests.TodoItem;
using TodoList.Application.Responses.TodoItem;
using TodoList.Application.TodoItems.Commands.AddTodoItem;
using TodoList.Application.TodoItems.Commands.DeleteTodoItem;
using TodoList.Application.TodoItems.Commands.UpdateTodoItem;
using TodoList.Application.TodoItems.Queries.GetTodoItem;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly ISender _sender;

    public TodoItemsController(ISender sender)
    {
        _sender = sender;
    }

    // GET: api/TodoItems/:todoItemId
    [HttpGet("{todoItemId}")]
    public async Task<ActionResult<GetTodoItemResponse>> GetTodoItem(Guid todoItemId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetTodoItemQuery(todoItemId), cancellationToken);

        return response == null ? NotFound() : Ok(response);
    }

    // PUT: api/TodoItems/:todoItemId
    [HttpPut("{todoItemId}")]
    public async Task<IActionResult> PutTodoItem([FromRoute] Guid todoItemId, [FromBody] UpdateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateTodoItemCommand(todoItemId, request), cancellationToken);

        return result.WasTodoItemFound ? NoContent() : NotFound();
    }

    // POST: api/TodoItems
    [HttpPost]
    public async Task<ActionResult<CreateTodoItemResponse>> PostTodoItem([FromBody] CreateTodoItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddTodoItemCommand(request), cancellationToken);
        if (result == null) return BadRequest("Could not find user with the given User ID.");

        var response = new GetTodoItemResponse(result.TodoItemId, request.Title, request.Description, request.DueDate, false);
        return CreatedAtAction(nameof(GetTodoItem), new { todoItemId = result.TodoItemId }, response);
    }


    // DELETE: api/TodoItems/:todoItemId
    [HttpDelete("{todoItemId}")]
    public async Task<IActionResult> DeleteTodoItem([FromRoute] Guid todoItemId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteTodoItemCommand(todoItemId), cancellationToken);
        if (!result.WasUserFound) return BadRequest("Could not find user with the given TodoItem ID");

        return NoContent();
    }
}