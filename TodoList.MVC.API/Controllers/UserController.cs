using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Repositories;
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.User;

namespace TodoList.MVC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly TodoContext _todoContext;
    private readonly IUserRepository _userRepository;

    public UserController(TodoContext todoContext, IUserRepository userRepository)
    {
        _todoContext = todoContext;
        _userRepository = userRepository;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<GetAllUsersResponse>> GetUsers()
    {
        var userList = await _userRepository.GetAllWithInclude();

        var users = from u in userList 
            select new GetUserResponse(u.Id, u.Email, u.Password, u.TodoItems.Select(t => t.Id).ToList(),
                u.Projects.Select(p => p.Id).ToList());

        return Ok(new GetAllUsersResponse(users.ToList()));
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] Guid id)
    {
        var user = await _userRepository.GetWithInclude(id);

        if (user == null) return NotFound();

        var response = new GetUserResponse(user.Id, user.Email, user.Password,
            user.TodoItems.Select(t => t.Id).ToList(), user.Projects.Select(p => p.Id).ToList());

        return Ok(response);
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
    {
        _todoContext
            .Entry(new User(id, request.Email, request.Password))
            .State = EntityState.Modified;

        try
        {
            await _todoContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> PostUser([FromBody] CreateUserRequest request)
    {
        var userId = Guid.NewGuid();

        _todoContext
            .Users
            .Add(new User(userId, request.Email, request.Password));
        await _todoContext
            .SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = userId },
            new CreateUserResponse(userId, request.Email, request.Password));
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var user = await _todoContext.Users.FindAsync(id);
        if (user == null) return NotFound();

        _todoContext.Users.Remove(user);
        await _todoContext.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(Guid id)
    {
        return (_todoContext.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}