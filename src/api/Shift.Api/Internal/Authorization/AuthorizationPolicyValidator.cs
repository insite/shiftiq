using System.Reflection;

using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class AuthorizationPolicyValidator : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthorizationPolicyValidator> _logger;

    public AuthorizationPolicyValidator(IServiceProvider serviceProvider, ILogger<AuthorizationPolicyValidator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var authorizationPolicyProvider = scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();

        var referencedPolicies = GetReferencedPolicies();

        var missingPolicies = new List<string>();

        foreach (var policyName in referencedPolicies)
        {
            try
            {
                var policy = await authorizationPolicyProvider.GetPolicyAsync(policyName);

                if (policy == null)
                {
                    missingPolicies.Add(policyName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking policy {PolicyName}", policyName);

                missingPolicies.Add(policyName);
            }
        }

        if (missingPolicies.Any())
        {
            var errorMessage = $"Missing authorization policies: {string.Join(", ", missingPolicies)}";

            _logger.LogError(errorMessage);

            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("All authorization policies validated successfully");
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private HashSet<string> GetReferencedPolicies()
    {
        var policies = new HashSet<string>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic
                && a.FullName != null
                && !a.FullName.StartsWith("System.")
                && !a.FullName.StartsWith("Microsoft."));

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    // Check type-level authorization attributes
                    AddPoliciesFromAttributes(policies, type.GetCustomAttributes<HybridAuthorizeAttribute>());
                    AddPoliciesFromAttributes(policies, type.GetCustomAttributes<BearerAuthorizeAttribute>());
                    AddPoliciesFromAttributes(policies, type.GetCustomAttributes<CookieAuthorizeAttribute>());
                    AddPoliciesFromAttributes(policies, type.GetCustomAttributes<SecretAuthorizeAttribute>());

                    // Check method-level authorization attributes
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        AddPoliciesFromAttributes(policies, method.GetCustomAttributes<AuthorizeAttribute>());
                    }

                    // Check property-level authorization attributes (less common but possible)
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        AddPoliciesFromAttributes(policies, property.GetCustomAttributes<AuthorizeAttribute>());
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Handle cases where some types can't be loaded
                _logger.LogWarning(ex, "Could not load some types from assembly {AssemblyName}", assembly.FullName);
            }
        }

        return policies;
    }

    private void AddPoliciesFromAttributes(HashSet<string> policies, IEnumerable<AuthorizeAttribute> attributes)
    {
        foreach (var attr in attributes)
        {
            if (!string.IsNullOrEmpty(attr.Policy))
                policies.Add(attr.Policy);
        }
    }
}
