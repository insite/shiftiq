using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Shift.Common;

namespace Shift.Api;

public class AuthenticationHelper
{
    private readonly HttpContext _context;

    private readonly string _scheme;

    public AuthenticationHelper(HttpContext context, string scheme)
    {
        _context = context;
        _scheme = scheme;
    }

    public bool AllowAnonymous()
    {
        var endpoint = _context.GetEndpoint();

        return endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;
    }

    public AuthenticateResult CreateTicket(ClaimsPrincipal? principal)
    {
        if (principal?.Identity?.AuthenticationType == null)
            return AuthenticateResult.Fail("The security principal Identity and AuthenticationType cannot be null");

        var ticket = new AuthenticationTicket(principal, principal.Identity.AuthenticationType);

        return AuthenticateResult.Success(ticket);
    }

    public AuthenticateResult Fail(string? message = null, ValidationFailure? validation = null)
    {
        var messages = new List<string>();

        if (_context.Items.ContainsKey("AuthenticationFailed"))
            messages.Add((string)_context.Items["AuthenticationFailed"]!);

        if (string.IsNullOrEmpty(message))
            messages.Add($"{_scheme} authentication skipped");
        else
            messages.Add(message);

        if (validation != null && validation.IsFailed())
        {
            foreach (var error in validation.Errors)
            {
                var errorMessage = error.Summary;
                if (error.Description != null)
                    errorMessage += ": " + error.Description;

                messages.Add(errorMessage);
            }
        }

        var failureMessage = string.Join(" ... ", messages.ToArray());

        _context.Items["AuthenticationFailed"] = failureMessage;

        return AuthenticateResult.Fail(failureMessage);
    }

    public bool IsFailed()
    {
        return _context.Items.ContainsKey("AuthenticationFailed");
    }
}
