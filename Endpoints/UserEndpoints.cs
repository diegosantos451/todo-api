using System.Security.Claims;

namespace api_todo.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users").RequireAuthorization();
        users.MapGet("/profile", GetProfile);
    }

    private static IResult GetProfile(ClaimsPrincipal user)
    {
        return Results.Ok(new
        {
            id = user.FindFirstValue(ClaimTypes.NameIdentifier),

            name = user.FindFirstValue(ClaimTypes.Name),

            email = user.FindFirstValue( ClaimTypes.Email),

            role = user.FindFirstValue(ClaimTypes.Role)
        });
    }
}