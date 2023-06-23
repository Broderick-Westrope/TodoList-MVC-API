using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Requests.User;
using TodoList.Application.Responses.User;
using TodoList.Application.Users.Commands.AddUser;
using TodoList.Application.Users.Commands.DeleteUser;
using TodoList.Application.Users.Commands.UpdateUser;
using TodoList.Application.Users.Queries.GetUser;
using TodoList.Application.Users.Queries.GetUserProjects;
using TodoList.Application.Users.Queries.GetUserTodoItems;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender) => _sender = sender;

    // GET: api/Users/:userId
    [HttpGet("{userId}")]
    public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetUserQuery(userId), cancellationToken);
        if (response == null) return NotFound();

        return Ok(response);
    }

    // GET: api/Users/:userId/Projects
    [HttpGet("{userId}/Projects")]
    public async Task<ActionResult<GetUserProjectsResponse>> GetUserProjects([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetUserProjectsQuery(userId), cancellationToken);
        if (response == null) return NotFound();

        return Ok(response);
    }

    // GET: api/Users/:userId/TodoItems
    [HttpGet("{userId}/TodoItems")]
    public async Task<ActionResult<GetUserTodoItemsResponse>> GetUserTodoItems([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetUserTodoItemsQuery(userId), cancellationToken);
        if (response == null) return NotFound();

        return Ok(response);
    }

    // PUT: api/Users/:userId
    [HttpPut("{userId}")]
    public async Task<IActionResult> PutUser([FromRoute] Guid userId, [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateUserCommand(userId, request), cancellationToken);

        return result.IsUserFound ? NoContent() : NotFound();
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<GetUserResponse>> PostUser([FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddUserCommand(request), cancellationToken);

        var response = new GetUserResponse(result.UserId, request.Email, request.Password);
        return CreatedAtAction(nameof(GetUser), new { userId = result.UserId }, response);
    }

    // DELETE: api/Users/:userId
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteUserCommand(userId), cancellationToken);

        return result.IsUserFound ? NoContent() : NotFound();
    }
}