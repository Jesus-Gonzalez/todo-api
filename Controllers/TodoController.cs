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
        public async Task<string> Post(Todo todo)
        {
            using (var ctx = new TodoContext()) {
                var id = Guid.NewGuid().ToString();
                todo.id = id;
                todo.lastModificationDate = ((DateTimeOffset)new DateTime()).ToUnixTimeSeconds();

                await ctx.Todos
                    .AddAsync(todo);

                var saved = await ctx.SaveChangesAsync() > 0;

                return id;
            }
        }

        [HttpPut("{id}")]
        public async Task<int> Put(string id, [FromBody] Todo todo)
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
                    entity.lastModificationDate = ((DateTimeOffset)new DateTime()).ToUnixTimeSeconds();
                }

                ctx.Todos.Update(entity);

                return await ctx.SaveChangesAsync();
            }
        }

        [HttpDelete("{id}")]
        public async Task<int> Delete(string id)
        {
            using (var ctx = new TodoContext()) {
                var entity = ctx.Todos.Remove(new Todo() {
                    id = id
                });

                return await ctx.SaveChangesAsync();
            }
        }
    }
}
