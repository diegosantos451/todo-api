using Microsoft.EntityFrameworkCore;
using api_todo.Users;
using api_todo.Todos;

namespace api_todo.Data
{
    public class TodoDb : DbContext
    {
        public TodoDb (DbContextOptions<TodoDb> options) : base(options) { }
        public DbSet<Todo> Todos => Set<Todo>();
        public DbSet<User> Users => Set<User>();
    }
}
/*
 * rodar as migrations para gerar o banco e as tabelas
 dotnet ef migrations add InitialCreate
dotnet ef database update*/