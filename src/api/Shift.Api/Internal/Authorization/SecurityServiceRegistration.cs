using Microsoft.AspNetCore.Authorization;

using Shift.Common;

namespace Shift.Api;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationPolicyValidation(this IServiceCollection services)
    {
        services.AddHostedService<AuthorizationPolicyValidator>();
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, SecuritySettings security)
    {
        services.AddSingleton<AuthorizerHolder>();

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

        services.AddAuthorization(options =>
        {
            var requirements = new RequirementList().BuildAuthorizationRequirements();

            foreach (var requirement in requirements)
            {
                options.AddPolicy(requirement.Policy, x => x.Requirements.Add(requirement));
            }
        });

        services.AddAuthorizationPolicyValidation();

        services.AddSingleton<IAuthorizationHandler, AuthorizationRequirementHandler>();

        services.AddHttpContextAccessor();

        services.AddSingleton<AuthorizerFactory>();

        services.AddTransient<IClaimConverter, ClaimConverter>();

        services.AddTransient<IPrincipalSearch, PrincipalSearch>();

        services.AddTransient<IShiftIdentityService, IdentityService>();

        return services;
    }
}