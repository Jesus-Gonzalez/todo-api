using System;
using Microsoft.EntityFrameworkCore;

namespace todo_api.Models
{
    public class TodoContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=todo.db");
        }
    }

    public class Todo {
        public string id { get; set; }
        public string name { get; set; }
        public bool? completed { get; set; } = false;
        public long lastModificationDate { get; set; }
    }
}