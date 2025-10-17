using System.Net;

namespace Engine.Scorm
{
    public class ScormAuthenticator
    {
        private readonly RequestDelegate _next;

        private const string UserNameHeader = "UserName";
        private const string PasswordHeader = "Password";

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

        private bool SkipAuthentication(HttpContext context)
        {
            var noAuthentication = new[] { "error", "status", "swagger", "version" };

            var path = context.Request.Path;

            return path.HasValue && noAuthentication.Any(x => path.Value.StartsWith("/" + x));
        }
    }
}