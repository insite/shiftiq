using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, SecuritySettings security)
    {
        services.AddSingleton<PermissionMatrixProvider>();
        services.AddHostedService<AuthorizationServiceInitializer>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = AuthenticationSchemeNames.Hybrid;
        })
        .AddPolicyScheme(AuthenticationSchemeNames.Hybrid, AuthenticationSchemeNames.Hybrid, options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var authorization = context.Request.Headers["Authorization"];

                var header = authorization.FirstOrDefault();

                var scheme = !string.IsNullOrEmpty(header) && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? AuthenticationSchemeNames.Bearer
                    : AuthenticationSchemeNames.Cookie;

                return scheme;
            };
        })
        .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(AuthenticationSchemeNames.Bearer, null)
        .AddScheme<CookieAuthenticationOptions, CookieAuthenticationHandler>(AuthenticationSchemeNames.Cookie, null)
        .AddScheme<SecretAuthenticationOptions, SecretAuthenticationHandler>(AuthenticationSchemeNames.Secret, null);

        services.AddAuthorizationRequirements();

        services.AddAuthorizationPolicyValidation();

        services.AddSingleton<IAuthorizationHandler, AuthorizationRequirementHandler>();

        services.AddHttpContextAccessor();

        services.AddSingleton<PermissionMatrixLoader>();

        services.AddTransient<IClaimConverter, ClaimConverter>();

        services.AddTransient<IPrincipalSearch, PrincipalSearch>();

        services.AddTransient<IShiftIdentityService, IdentityService>();

        return services;
    }

    private static IServiceCollection AddAuthorizationRequirements(this IServiceCollection services)
    {
        // Create and register the requirements collection for the AuthorizationServiceInitializer.

        var requirements = CreateAuthorizationRequirements();

        services.AddSingleton(requirements);

        services.AddAuthorization(options =>
        {
            foreach (var requirement in requirements)
            {
                options.AddPolicy(requirement.Policy, x => x.Requirements.Add(requirement));
            }
        });

        return services;
    }

    private static IServiceCollection AddAuthorizationPolicyValidation(this IServiceCollection services)
    {
        services.AddHostedService<AuthorizationPolicyValidator>();

        return services;
    }

    /// <summary>
    /// Creates the list of authorization requirements for the API
    /// </summary>
    /// <remarks>
    /// All authorization requirements are defined in the Policies constant.
    /// </remarks>
    private static List<AuthorizationRequirement> CreateAuthorizationRequirements()
    {
        var resourceReflector = new ResourceReflector();

        var resources = resourceReflector.BuildResourceList(typeof(Policies));

        var requirementBuilder = new AuthorizationRequirementBuilder();

        var requirements = requirementBuilder.BuildAuthorizationRequirements(resources);

        return requirements;
    }
}