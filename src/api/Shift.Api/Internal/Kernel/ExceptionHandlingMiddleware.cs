using System.Net;

using Microsoft.AspNetCore.Http.Extensions;

namespace Shift.Api;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMonitor _monitor;

    public ExceptionHandlingMiddleware(RequestDelegate next, IMonitor monitor)
    {
        _next = next;
        _monitor = monitor;
    }

    public static Problem ReportUnexpectedProblem(Exception ex, string? doingWhat, HttpContext context, IMonitor monitor)
    {
        var message = "Our team is looking into the problem for you. " +
            ex.GetFormattedMessages();

        var requestUrl = context?.Request?.GetDisplayUrl();

        if (requestUrl != null)
            message = $"HTTP endpoint {requestUrl} returned an unexpected response. " + message;

        if (doingWhat != null)
            message = $"An error occurred {doingWhat}. " + message;

        var uri = monitor?.Error(ex);

        var problem = ProblemFactory.InternalServerError(message, uri);

        return problem;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Items.TryGetValue("AuthenticationFailed", out var item))
            {
                var failureMessage = item as string;

                if (!string.IsNullOrWhiteSpace(failureMessage))
                {
                    await AuthenticationFailedAsync(context, failureMessage);
                }
            }
        }
        catch (OperationCanceledException)
        {
            await WriteResponseAsync(context, ProblemFactory.RequestTimeout());
        }
        catch (Exception ex)
        {
            var problem = ReportUnexpectedProblem(ex, null, context, _monitor);

            await WriteResponseAsync(context, problem);
        }
    }

    private async Task WriteResponseAsync(HttpContext context, Problem problem)
    {
        context.Response.Clear();

        context.Response.ContentType = "application/problem+json";

        context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = problem.Serialize();

        await context.Response.WriteAsync(json);

        await context.Response.CompleteAsync();
    }

    private async Task AuthenticationFailedAsync(HttpContext context, string message)
    {
        var response = context.Response;

        if (!response.HasStarted)
        {
            response.StatusCode = 401;

            var key = $"X-Authentication-Failed";

            if (response.Headers.ContainsKey(key))
                response.Headers[key] += message;
            else
                response.Headers.Append(key, message);

            response.ContentType = "application/json";

            var error = new Error
            {
                Summary = $"Authentication Failed",
                Description = message
            };

            await response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
        }
    }
}
