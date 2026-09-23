using api_todo.Data;
using api_todo.Users;
using Microsoft.EntityFrameworkCore;

namespace api_todo.Endpoints
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this WebApplication app)
        {
            var admin = app.MapGroup("/admin").RequireAuthorization(policy => policy.RequireRole("admin"));
            admin.MapGet("/users", GetAllUsers);
            admin.MapGet("/users/{id}", GetUser);
        }

        public static async Task<IResult> GetAllUsers(TodoDb db)
        {
            var users = await db.Users.Select(x => new UserDTO(x)).ToListAsync();

            return TypedResults.Ok(users);

        }

        public static async Task<IResult> GetUser(TodoDb db, int id)
        {
            var user = await db.Users.Include(u => u.Todos).FirstOrDefaultAsync(u => u.Id == id);
            
            if(user is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(new AdminUserDto(user));

        }
    }
}
