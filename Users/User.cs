using api_todo.Todos;

namespace api_todo.Users;

public class User
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? HashPassword { get; set; }
    public string Role { get; set; } = "user";
    public List<Todo> Todos { get; set; } = new();
}
