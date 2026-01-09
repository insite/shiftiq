using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly PermissionMatrixProvider _permissionMatrixProvider;
    private readonly IShiftIdentityService _identityService;

    public AuthorizationRequirementHandler(PermissionMatrixProvider permissionMatrixProvider, IShiftIdentityService identityService)
    {
        _permissionMatrixProvider = permissionMatrixProvider;
        _identityService = identityService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        var matrix = _permissionMatrixProvider.Matrix;

        var identity = _identityService.GetPrincipal();

        var isAuthenticated = identity != null && identity.IsAuthenticated;

        if (!isAuthenticated)
        {
            context.Fail(new AuthorizationFailureReason(this, "User is not authenticated"));
        }

        if (identity != null && identity.Organization.Slug != null)
        {
            var organization = identity.Organization.Slug;

            if (!matrix.IsAllowed(organization, requirement.Policy, identity.Roles))
            {
                context.Fail(new AuthorizationFailureReason(this, "User is not granted permission"));
            }
        }

        if (!context.HasFailed)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}