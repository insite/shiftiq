using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly AuthorizerFactory _authorizer;

    public AuthorizationRequirementHandler(AuthorizerFactory authorizer)
    {
        _authorizer = authorizer;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        var identity = context.User.Identity;

        var isAuthenticated = identity != null && identity.IsAuthenticated;

        if (!isAuthenticated)
            context.Fail(new AuthorizationFailureReason(this, "User is not authenticated"));

        // TODO: if (_authorizer.IsGranted(requirement))

        if (!context.HasFailed)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}