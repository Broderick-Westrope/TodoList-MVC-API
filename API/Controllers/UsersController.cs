using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Requests.User;
using TodoList.Application.Responses.Project;
using TodoList.Application.Responses.TodoItem;
using TodoList.Application.Responses.User;
using TodoList.Application.Users.Queries;
using TodoList.Domain;
using TodoList.Domain.Entities;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository, ISender sender)
    {
        _userRepository = userRepository;
        _sender = sender;
    }

    // GET: api/Users/:userId
    [HttpGet("{userId}")]
    public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetUserQuery { Id = userId }, cancellationToken);
        if (response == null) return NotFound();

        return Ok(response);
    }

    // GET: api/Users/:userId/Projects
    [HttpGet("{userId}/Projects")]
    public async Task<ActionResult<GetUserProjectsResponse>> GetUserProjects([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(userId, cancellationToken);
        if (user == null) return NotFound();

        var projects = from p in user.Projects
            select new GetProjectResponse(p.Id, p.Title);

        return Ok(new GetUserProjectsResponse(projects.ToList()));
    }

    // GET: api/Users/:userId/TodoItems
    [HttpGet("{userId}/TodoItems")]
    public async Task<ActionResult<GetUserTodoItemsResponse>> GetUserTodoItems([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(userId, cancellationToken);
        if (user == null) return NotFound();

        var todoItems = from t in user.TodoItems
            select new GetTodoItemResponse(t.Id, t.Title, t.Description, t.DueDate, t.IsCompleted);

        return Ok(new GetUserTodoItemsResponse(todoItems.ToList()));
    }

    // PUT: api/Users/:userId
    [HttpPut("{userId}")]
    public async Task<IActionResult> PutUser([FromRoute] Guid userId, [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        _userRepository.Update(new UserAggregate(userId, request.Email, request.Password));

        try
        {
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _userRepository.Get(userId, cancellationToken) == null)
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> PostUser([FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var userAggregate = new UserAggregate(userId, request.Email, request.Password);

        await _userRepository.Add(userAggregate, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var response = userAggregate.Adapt<CreateUserResponse>();
        return CreatedAtAction(nameof(GetUser), new { userId }, response);
    }

    // DELETE: api/Users/:userId
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(userId, cancellationToken);
        if (user == null) return NotFound();

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}