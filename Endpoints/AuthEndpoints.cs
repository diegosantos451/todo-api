using api_todo.Data;
using api_todo.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_todo.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var auth = app.MapGroup("/auth");

            auth.MapPost("/register", Register);
            auth.MapPost("/login", Login);

        }

        private static async Task<IResult> Register(RegisterDto request, TodoDb db)
        {
            var emailExists = await db.Users.AnyAsync(u => u.Email == request.Email);

            if (emailExists)
            {
                return Results.BadRequest("E-mail já cadastrado.");
            }

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Usuário cadastrado com sucesso." });
        }

        private static async Task<IResult> Login(LoginDto request, TodoDb db, IConfiguration configuration)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword);

            if (!passwordValid)
            {
                return Results.Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),

                new Claim(ClaimTypes.Name,user.Name),

                new Claim(ClaimTypes.Email,user.Email),

                new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new
            {
                token = tokenString
            });
        }
    }
}
