using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API.Models;
using Task = TodoList.MVC.API.Models.Task;

namespace TodoList.MVC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TodoContext _context;

        public TaskController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/Task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
        {
          if (_context.Tasks == null)
          {
              return NotFound();
          }
          return await _context.Tasks.ToListAsync();
        }

        // GET: api/Task/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(Guid id)
        {
          if (_context.Tasks == null)
          {
              return NotFound();
          }
          var task = await _context.Tasks.FindAsync(id);

          if (task == null)
          {
              return NotFound();
          }

          return task;
        }

        // PUT: api/Task/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(Guid id, Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Task
        [HttpPost]
        public async Task<ActionResult<Task>> PostTask(string title, string description = "", DateTime dueDate = default)
        {
          if (_context.Tasks == null)
          {
              return Problem("Entity set 'TodoContext.Tasks'  is null.");
          }

          var task = new Task(UserController.CurrentUserId, title, description, dueDate);
          
          _context.Tasks.Add(task);
          await _context.SaveChangesAsync();

          return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // DELETE: api/Task/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            if (_context.Tasks == null)
            {
                return NotFound();
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(Guid id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
