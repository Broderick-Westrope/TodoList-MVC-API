using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;

namespace TodoList.MVC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TodoContext _context;

        public UserController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
          var user = await _context.Users.FindAsync(id);

          if (user == null)
          {
              return NotFound();
          }

          return user;
        }

        // Create User
        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'TodoContext.Users'  is null.");
          }
          _context.Users.Add(user);
          try
          {
              await _context.SaveChangesAsync();
          }
          catch (DbUpdateException)
          {
              if (UserExists(user.Email))
              {
                  return Conflict();
              }
              else
              {
                  throw;
              }
          }

          return CreatedAtAction("GetUser", new { id = user.Email }, user);
        }

        // Delete User
        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Email == id)).GetValueOrDefault();
        }
    }
}
