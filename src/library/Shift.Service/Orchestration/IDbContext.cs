using System.Reflection;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Constant;

namespace Shift.Service
{
    internal interface IDbContext
    {
    }

    public class PrincipalSearch : IPrincipalSearch
    {
        private const string DefaultLanguage = "en";
        private const string DefaultTimeZone = "Mountain Standard Time";

        private readonly IClaimConverter _converter;
        private readonly IDbContextFactory<TableDbContext> _dbContextFactory;

        public PrincipalSearch(IDbContextFactory<TableDbContext> dbContextFactory, IClaimConverter converter)
        {
            _dbContextFactory = dbContextFactory;
            _converter = converter;
        }

        public IShiftPrincipal? GetPrincipal(string secret)
        {
            using var db = _dbContextFactory.CreateDbContext();

            var personSecret = db.QPersonSecret.FirstOrDefault(x => x.SecretValue == secret);

            if (personSecret == null)
                return null;

            var person = db.QPerson.AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefault(x => x.PersonIdentifier == personSecret.PersonIdentifier);

            if (person?.User == null)
                return null;

            var principal = new Principal();

            principal.User = new Actor
            {
                Email = person.User.Email,
                Name = person.User.FullName,
                Language = person.Language ?? DefaultLanguage,
                TimeZone = person.User.TimeZone ?? DefaultTimeZone
            };

            principal.User.Identifier = person.UserIdentifier;

            principal.Organization = new Model { Identifier = person.OrganizationIdentifier };

            principal.Person = new Model { Identifier = person.PersonIdentifier };

            var partition = db.TPartitionSetting.FirstOrDefault(x => x.SettingName == "Partition:Identifier");

            if (partition != null)
                principal.Partition = new Model { Identifier = Guid.Parse(partition.SettingValue) };

            var organizations = new List<Guid> { principal.Organization.Identifier };

            if (principal.Partition != null)
                organizations.Add(principal.Partition.Identifier);

            var roles = db.QMembership
                .Where(x => x.UserIdentifier == principal.User.Identifier
                    && x.Group != null
                    && x.Group.GroupType == "Role"
                    && organizations.Contains(x.Group.OrganizationIdentifier))
                .Select(x => x.Group)
                .ToList();

            foreach (var role in roles)
            {
                principal.Roles.Add(CreateRole(role!.GroupIdentifier, role.GroupName));
            }

            principal.Roles.Add(CreateRole(principal.User.Identifier, principal.User.Name));

            return principal;
        }

        private Role CreateRole(Guid identifier, string name)
        {
            return new Role { Identifier = identifier, Name = name };
        }

        public IShiftPrincipal? GetPrincipal(JwtRequest request, string ipAddress, bool isWhitelisted, int? lifetime, List<string> errors)
        {
            IShiftPrincipal? principal = null;

            if (isWhitelisted)
                principal = _converter.ToSentinel(request);

            if (principal == null)
                principal = GetPrincipal(request.Secret);

            if (principal == null)
                return null;

            principal.IPAddress = ipAddress;

            principal.Claims.Lifetime = _converter.CalculateLifetime(principal.Claims.Lifetime, request.Lifetime, lifetime);

            return principal;
        }

        private static MemoryCache<Guid, string> SecretCache = new MemoryCache<Guid, string>();

        public string? GetSecret(IEnumerable<Claim> claims)
        {
            var principal = _converter.ToPrincipal(claims);

            var personId = principal.Person?.Identifier ?? Guid.Empty;

            if (personId != Guid.Empty && SecretCache.Exists(personId))
                return SecretCache[personId];

            using var db = _dbContextFactory.CreateDbContext();

            var person = db.QPerson.SingleOrDefault(x => x.UserIdentifier == principal.User.Identifier
                && x.OrganizationIdentifier == principal.Organization.Identifier);

            if (person == null)
                return null;

            var personSecret = db.QPersonSecret.SingleOrDefault(x => x.PersonIdentifier == person.PersonIdentifier
                && x.SecretType == SecretType.Authentication
                && x.SecretName == SecretName.ShiftClientSecret);

            var secret = personSecret?.SecretValue;

            if (secret != null)
                SecretCache.Add(personId, secret);

            return secret;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QueryServiceRegistration
    {
        public static IServiceCollection AddQueries(this IServiceCollection services, Assembly assembly)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a =>
                {
                    try
                    {
                        // Trigger type load to force early failure
                        _ = a.GetTypes();
                        return true;
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        // Log or break here to see which assembly causes the error
                        Console.WriteLine($"Failed to load types from: {a.FullName}");
                        foreach (var loaderEx in ex.LoaderExceptions)
                            Console.WriteLine($" - {loaderEx?.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Non-typeload failure from {a.FullName}: {ex.Message}");
                        return false;
                    }
                })
                .ToArray();

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo<IQueryRunner>())
                .As<IQueryRunner>()
                .WithScopedLifetime());

            services.AddScoped<QueryDispatcher>();

            services.AddSingleton(x => new QueryTypeCollection(assembly));

            return services;
        }
    }
}