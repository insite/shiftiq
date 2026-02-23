using System.Reflection;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shift.Api;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        List<T> GetAllAttributes<T>() where T : Attribute
        {
            var methodAttributes = context.MethodInfo.GetCustomAttributes<T>().ToList();
            var controllerAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes<T>().ToList() ?? new List<T>();
            return methodAttributes.Concat(controllerAttributes).ToList();
        }

        var allHybridAuthorizeAttributes = GetAllAttributes<HybridAuthorizeAttribute>();
        var allBearerAuthorizeAttributes = GetAllAttributes<BearerAuthorizeAttribute>();
        var allCookieAuthorizeAttributes = GetAllAttributes<CookieAuthorizeAttribute>();
        var allSecretAuthorizeAttributes = GetAllAttributes<SecretAuthorizeAttribute>();
        var allPermissionAttributes = GetAllAttributes<RequirePermissionAttribute>();

        var allAuthorizationAttributes = allHybridAuthorizeAttributes.Cast<Attribute>()
            .Concat(allBearerAuthorizeAttributes.Cast<Attribute>())
            .Concat(allCookieAuthorizeAttributes.Cast<Attribute>())
            .Concat(allSecretAuthorizeAttributes.Cast<Attribute>())
            .Concat(allPermissionAttributes.Cast<Attribute>())
            .ToList();

        // Initialize OpenApiOperation security requirements
        var securityRequirements = new List<OpenApiSecurityRequirement>();

        // Check which schemes are used by permission attributes
        var permissionSchemes = allPermissionAttributes
            .Select(attr => attr.AuthenticationSchemes)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        var hasHybridPermission = permissionSchemes.Contains(AuthenticationSchemeNames.Hybrid);
        var hasBearerPermission = permissionSchemes.Contains(AuthenticationSchemeNames.Bearer);
        var hasCookiePermission = permissionSchemes.Contains(AuthenticationSchemeNames.Cookie);
        var hasSecretPermission = permissionSchemes.Contains(AuthenticationSchemeNames.Secret);

        // Add security requirements based on attribute types
        if (allHybridAuthorizeAttributes.Any() || allBearerAuthorizeAttributes.Any() || hasHybridPermission || hasBearerPermission)
        {
            securityRequirements.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = []
            });
        }

        if (allHybridAuthorizeAttributes.Any() || allCookieAuthorizeAttributes.Any() || hasHybridPermission || hasCookiePermission)
        {
            securityRequirements.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Cookie"
                    }
                }] = []
            });
        }

        if (allSecretAuthorizeAttributes.Any() || hasSecretPermission)
        {
            securityRequirements.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Secret"
                    }
                }] = []
            });
        }

        // Set the security requirements on the operation
        operation.Security = securityRequirements;

        // Add custom extensions with policy information
        var hybridPolicies = allHybridAuthorizeAttributes
            .Select(attr => attr.Policy)
            .Where(policy => !string.IsNullOrEmpty(policy))
            .Distinct()
            .ToList();

        var bearerPolicies = allBearerAuthorizeAttributes
            .Select(attr => attr.Policy)
            .Where(policy => !string.IsNullOrEmpty(policy))
            .Distinct()
            .ToList();

        var cookiePolicies = allCookieAuthorizeAttributes
            .Select(attr => attr.Policy)
            .Where(policy => !string.IsNullOrEmpty(policy))
            .Distinct()
            .ToList();

        var secretPolicies = allSecretAuthorizeAttributes
            .Select(attr => attr.Policy)
            .Where(policy => !string.IsNullOrEmpty(policy))
            .Distinct()
            .ToList();

        var permissionPolicies = allPermissionAttributes
            .Select(attr => attr.Policy)
            .Where(policy => !string.IsNullOrEmpty(policy))
            .Distinct()
            .ToList();

        // Combine all policies and add to extensions
        var allPolicies = hybridPolicies
            .Concat(bearerPolicies)
            .Concat(cookiePolicies)
            .Concat(secretPolicies)
            .Concat(permissionPolicies)
            .Distinct()
            .ToList();

        if (allPolicies.Any())
        {
            var policyArray = new OpenApiArray();
            foreach (var policy in allPolicies)
            {
                policyArray.Add(new OpenApiString(policy));
            }
            operation.Extensions.Add("x-authorization-policies", policyArray);
        }

        // Add extension with attribute types present
        var attributeTypes = new OpenApiArray();
        if (allHybridAuthorizeAttributes.Any() || hasHybridPermission)
            attributeTypes.Add(new OpenApiString("Hybrid"));
        if (allBearerAuthorizeAttributes.Any() || hasBearerPermission)
            attributeTypes.Add(new OpenApiString("Bearer"));
        if (allCookieAuthorizeAttributes.Any() || hasCookiePermission)
            attributeTypes.Add(new OpenApiString("Cookie"));
        if (allSecretAuthorizeAttributes.Any() || hasSecretPermission)
            attributeTypes.Add(new OpenApiString("Secret"));

        if (attributeTypes.Any())
        {
            operation.Extensions.Add("x-authorization-types", attributeTypes);
        }
    }
}
