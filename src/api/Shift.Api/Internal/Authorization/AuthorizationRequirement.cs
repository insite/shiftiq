using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class AuthorizationRequirement : IAuthorizationRequirement
{
    public string Resource { get; }
    public AccessType? Type { get; }
    public int AccessValue { get; }

    public AuthorizationRequirement(string policy)
    {
        var parts = policy.Split(':');

        Resource = parts[0];

        if (parts.Length >= 3 && Enum.TryParse<AccessType>(parts[1], true, out var type))
        {
            Type = type;
            AccessValue = int.Parse(parts[2]);
        }
    }

    public bool Evaluate(PermissionList list, IEnumerable<string> roles)
    {
        if (Type == null)
            return list.IsAllowed(Resource, roles);

        return Type switch
        {
            AccessType.Feature
                => list.IsAllowed(Resource, roles, (FeatureAccess)AccessValue),

            AccessType.Data
                => list.IsAllowed(Resource, roles, (DataAccess)AccessValue),

            AccessType.Authority
                => list.IsAllowed(Resource, roles, (AuthorityAccess)AccessValue),

            _ => false
        };
    }
}
