using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly PermissionCache _permissionCache;
    private readonly IPrincipalProvider _identityService;

    public AuthorizationRequirementHandler(
        PermissionCache permissionCache,
        IPrincipalProvider identityService)
    {
        _permissionCache = permissionCache;
        _identityService = identityService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        var identity = _identityService.GetPrincipal();

        if (identity?.IsAuthenticated != true)
            return Fail(context, "User is not authenticated");

        var organization = identity.Organization?.Slug;
        if (string.IsNullOrEmpty(organization))
            return Fail(context, "User has no organization");

        var matrix = _permissionCache.Matrix;
        if (!matrix.TryGetPermissions(organization, out var permissions))
            return Fail(context, $"Organization not found: {organization}");

        var roles = identity.Roles.Select(r => r.Name);

        if (requirement.Evaluate(permissions, roles))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        return Fail(context, $"Permission denied: {requirement.Resource}");
    }

    private Task Fail(AuthorizationHandlerContext context, string reason)
    {
        context.Fail(new AuthorizationFailureReason(this, reason));
        return Task.CompletedTask;
    }
}