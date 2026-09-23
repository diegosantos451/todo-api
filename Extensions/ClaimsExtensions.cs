using System.Security.Claims;

namespace api_todo.Extensions
{
    public static class ClaimsExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Usuário não autenticado.");

            return long.Parse(userId);
        }
    }
}
