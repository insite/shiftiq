namespace Shift.Api
{
    internal static class HttpContextExtensions
    {
        public static string? GetBearerToken(this IHttpContextAccessor httpContextAccessor)
        {
            var authHeader = httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader))
                return null;

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            return authHeader.Substring("Bearer ".Length).Trim();
        }
    }
}
