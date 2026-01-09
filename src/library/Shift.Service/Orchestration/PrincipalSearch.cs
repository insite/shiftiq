using System.Reflection;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Constant;
using Shift.Service.Directory;

namespace Shift.Service;

public class PrincipalSearch : IPrincipalSearch
{
    private const string DefaultLanguage = "en";
    private const string DefaultTimeZone = "Mountain Standard Time";

    private readonly IClaimConverter _converter;
    private readonly IDbContextFactory<TableDbContext> _dbContextFactory;

    private static MemoryCache<Guid, string> SecretCache = new MemoryCache<Guid, string>();

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
            .Include(x => x.Organization)
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

        if (person.Organization != null)
        {
            principal.Organization = new Model
            {
                Identifier = person.OrganizationIdentifier,
                Name = person.Organization.CompanyName,
                Slug = person.Organization.OrganizationCode
            };
        }

        principal.Person = new Model
        {
            Identifier = person.PersonIdentifier,
            Name = person.FullName
        };

        AddAuthority(principal, person);

        var partitionSettings = db.TPartitionSetting.ToList();
        var partitionId = partitionSettings.FirstOrDefault(x => x.SettingName == "Partition:Identifier")?.SettingValue;
        var partitionSlug = partitionSettings.FirstOrDefault(x => x.SettingName == "Partition:Slug")?.SettingValue;
        if (partitionId != null && partitionSlug != null)
            principal.Partition = new Model
            {
                Identifier = Guid.Parse(partitionId),
                Slug = partitionSlug
            };

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
            principal.Roles.Add(new Role(role!.GroupName, role!.GroupIdentifier));
        }

        principal.Roles.Add(new Role(principal.User.Name, principal.User.Identifier));

        return principal;
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

    private void AddAuthority(Principal principal, ISystemRoles person)
    {
        var (access, roles) = GetAuthority(person);

        principal.Authority = access;
        principal.Roles.AddRange(roles);
    }

    public static (AuthorityAccess, List<Role>) GetAuthority(ISystemRoles person)
    {
        var access = AuthorityAccess.None;
        var roles = new List<Role>();

        if (person.IsAdministrator)
        {
            access |= AuthorityAccess.Administrator;
            roles.Add(new Role(SystemRole.Administrator));
        }

        if (person.IsDeveloper)
        {
            access |= AuthorityAccess.Developer;
            roles.Add(new Role(SystemRole.Developer));
        }

        if (person.IsLearner)
        {
            access |= AuthorityAccess.Learner;
            roles.Add(new Role(SystemRole.Learner));
        }

        if (person.IsOperator)
        {
            access |= AuthorityAccess.Operator;
            roles.Add(new Role(SystemRole.Operator));
        }

        return (access, roles);
    }
}