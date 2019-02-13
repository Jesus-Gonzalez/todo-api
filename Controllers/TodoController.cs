using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_api.Models;

namespace todo_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Todo>> GetTodosAsync()
        {
            using (var ctx = new TodoContext()) {
                return await ctx.Todos.ToArrayAsync();
            }
        }

        [HttpGet("{id}")]
        public async Task<Todo> GetTodoByIdAsync(string id)
        {
            using (var ctx = new TodoContext()) {
                return await ctx.Todos
                    .Where(todo => todo.id == id)
                    .FirstOrDefaultAsync();
            }
        }

        [HttpPost]
        public async Task<object> Post([FromBody] Todo todo)
        {
            using (var ctx = new TodoContext()) {
                var id = Guid.NewGuid().ToString();
                todo.id = id;
                todo.lastModificationDate = DateTimeOffset.Now.ToUnixTimeSeconds();

                await ctx.Todos
                    .AddAsync(todo);

                var saved = await ctx.SaveChangesAsync() > 0;

                return new {
                    todo = todo,
                    success = saved
                };
            }
        }

        [HttpPut]
        public async Task<object> Put([FromBody] Todo todo)
        {
            using (var ctx = new TodoContext()) {
                ctx.Todos.Update(todo);

                return new {
                    success = await ctx.SaveChangesAsync() > 0
                };
            }
        }

        [HttpPatch("{id}")]
        public async Task<object> Patch(string id, [FromBody] Todo todo)
        {
            using (var ctx = new TodoContext()) {
                var entity = await ctx.Todos.SingleOrDefaultAsync();
                if (todo.name != null) {
                    entity.name = todo.name;
                }

                if (todo.completed.HasValue) {
                    entity.completed = todo.completed.Value;
                }

                if (todo.name != null || todo.completed.HasValue) {
                    entity.lastModificationDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                }

                ctx.Todos.Update(entity);

                var saved = await ctx.SaveChangesAsync() > 0;

                return new {
                    success = saved
                };
            }
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete(string id)
        {
            using (var ctx = new TodoContext()) {
                var entity = ctx.Todos.Remove(new Todo() {
                    id = id
                });

                return new {
                    success = await ctx.SaveChangesAsync() > 0
                };
            }
        }
    }
}
