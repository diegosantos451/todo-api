using api_todo.Data;
using api_todo.Todos;
using api_todo.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_todo.Endpoints
{
    public static class TodoEndpoints
    {
        public static void MapTodoEndpoints(this WebApplication app)
        {
            var todoItems = app.MapGroup("/todoitems").RequireAuthorization();

            todoItems.MapGet("/", GetAllTodos);
            todoItems.MapGet("/complete", GetCompleteTodos);
            todoItems.MapGet("/{id}", GetTodo);
            todoItems.MapPost("/", CreateTodo);
            todoItems.MapPut("/{id}", UpdateTodo);
            todoItems.MapPatch("/{id}", PatchTodo);
            todoItems.MapDelete("/{id}", DeleteTodo);

            //métodos a serem chamados nos endpoints no lugar das funções lâmbidas:

            static async Task<IResult> GetAllTodos(TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                var todos = await db.Todos.Where(t => t.UserId == userId).Select(t => new TodoItemDTO(t)).ToListAsync();
                return TypedResults.Ok(todos);
            }

            static async Task<IResult> GetCompleteTodos(TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete && userId == t.UserId).Select(x => new TodoItemDTO(x)).ToListAsync());
            }

            static async Task<IResult> GetTodo(int id, TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                var todo = await db.Todos.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);
                if (todo == null) return TypedResults.NotFound();
                return TypedResults.Ok(new TodoItemDTO(todo));
                
                    
            }

            static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                var todoItem = new Todo
                {
                    IsComplete = todoItemDTO.IsComplete,
                    Name = todoItemDTO.Name,
                    UserId = userId
                };

                db.Todos.Add(todoItem);
                await db.SaveChangesAsync();

                todoItemDTO = new TodoItemDTO(todoItem);

                return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
            }

            static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                var todo = await db.Todos.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);
                
                if (todo is null) return TypedResults.NotFound();

                todo.Name = todoItemDTO.Name;
                todo.IsComplete = todoItemDTO.IsComplete;

                await db.SaveChangesAsync();

                return TypedResults.NoContent();
            }

            static async Task<IResult> PatchTodo(int id, TodoPatchDTO inputTodo, TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                var todo = await db.Todos.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

                if (todo is null) return TypedResults.NotFound();

                if (inputTodo.Name is not null) todo.Name = inputTodo.Name;
                if (inputTodo.IsComplete is not null) todo.IsComplete = inputTodo.IsComplete.Value;

                await db.SaveChangesAsync();

                return TypedResults.NoContent();
            }

            static async Task<IResult> DeleteTodo(int id, TodoDb db, ClaimsPrincipal user)
            {
                var userId = user.GetUserId();
                var todo = await db.Todos.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

                if (todo is null) return TypedResults.NotFound();
         
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }   
        }
    }
}
