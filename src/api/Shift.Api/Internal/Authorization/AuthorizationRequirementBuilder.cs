namespace Shift.Api;

public class AuthorizationRequirementBuilder
{
    public List<AuthorizationRequirement> BuildAuthorizationRequirements(List<Resource> resources)
    {
        var list = resources
            .Select(x => new AuthorizationRequirement(x.Name))
            .ToList();

        var aliases = resources
            .SelectMany(x => x.Aliases);

        foreach (var alias in aliases)
            list.Add(new AuthorizationRequirement(alias));

        return list;
    }
}
