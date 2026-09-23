using api_todo.Todos;

namespace api_todo.Users
{
    public class AdminUserDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public List<TodoItemDTO> Todos { get; set; } = [];

        public AdminUserDto() { }
        public AdminUserDto(User user) => (Id, Name, Email, Role, Todos) = ( user.Id, user.Name, user.Email, user.Role, user.Todos.Select(x => new TodoItemDTO(x)).ToList());

    }
}
