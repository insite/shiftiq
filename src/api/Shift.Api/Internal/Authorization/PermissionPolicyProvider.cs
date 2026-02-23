using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Shift.Api;

/// <summary>
/// Provides authorization policies for permission-based policy names.
/// </summary>
/// <remarks>
/// Parses policy names in the format:
/// - "resource" (basic access)
/// - "resource:authority:N" (authority access)
/// - "resource:data:N" (data access)
/// - "resource:feature:N" (feature access)
/// Where N is the integer value of the access enum.
/// </remarks>
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => await _fallbackProvider.GetDefaultPolicyAsync();

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => await _fallbackProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if this looks like a permission policy (contains resource path)
        if (IsPermissionPolicy(policyName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new AuthorizationRequirement(policyName))
                .Build();

            return policy;
        }

        // Fall back to default provider for other policies
        return await _fallbackProvider.GetPolicyAsync(policyName);
    }

    private static bool IsPermissionPolicy(string policyName)
    {
        // Permission policies contain "/" (resource path) or ":" (access type separator)
        return policyName.Contains('/') || policyName.Contains(':');
    }
}
