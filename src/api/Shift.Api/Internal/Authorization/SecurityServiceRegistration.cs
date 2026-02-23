using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, SecuritySettings security)
    {
        services.AddSingleton<PermissionMatrixLoader>();

        services.AddSingleton<PermissionCache>();

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

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        services.AddSingleton<IAuthorizationHandler, AuthorizationRequirementHandler>();

        services.AddHttpContextAccessor();

        services.AddTransient<IClaimConverter, ClaimConverter>();

        services.AddTransient<IPrincipalSearch, PrincipalSearch>();

        services.AddTransient<IPrincipalProvider, PrincipalProvider>();

        return services;
    }
}