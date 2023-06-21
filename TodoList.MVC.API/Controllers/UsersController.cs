using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.DbModels;
using TodoList.MVC.API.Repositories;
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.Project;
using TodoList.MVC.API.Responses.TodoItem;
using TodoList.MVC.API.Responses.User;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/Users/:userId
    [HttpGet("{userId}")]
    public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetWithInclude(userId, cancellationToken);
        if (user == null) return NotFound();

        var response = new GetUserResponse(user.Id, user.Email, user.Password,
            user.TodoItems!.Select(t => t.Id).ToList(), user.Projects!.Select(p => p.Id).ToList());

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