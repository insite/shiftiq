using System.Net;

namespace Shift.Hub.ScormCloud
{
    /// <summary>
    /// Requires UserName/Password headers for SCORM endpoints. Applied only to /scorm paths via
    /// <c>UseWhen</c> in <c>Program.cs</c>, and internally exempts diagnostic/docs paths.
    /// </summary>
    public class ScormAuthenticator
    {
        private readonly RequestDelegate _next;

        private const string UserNameHeader = "UserName";
        private const string PasswordHeader = "Password";

        private static readonly string[] ExemptPrefixes =
        {
            "/scorm/error",
            "/scorm/status",
            "/scorm/health",
            "/scorm/swagger",
            "/scorm/version"
        };

        public ScormAuthenticator(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!SkipAuthentication(context))
            {
                if (!context.Request.Headers.TryGetValue(UserNameHeader, out var userName) ||
                    !context.Request.Headers.TryGetValue(PasswordHeader, out var password))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Missing credentials.");
                    return;
                }

                context.Items["AuthenticatedUserName"] = userName.ToString();
                context.Items["AuthenticatedPassword"] = password.ToString();
            }

            await _next(context);
        }

        private static bool SkipAuthentication(HttpContext context)
        {
            var path = context.Request.Path;
            if (!path.HasValue)
                return true;

            return ExemptPrefixes.Any(x => path.Value.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
