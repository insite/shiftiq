namespace Shift.Api;

public class RequirementList
{
    public List<Requirement> Items { get; set; } = [];

    public List<AuthorizationRequirement> BuildAuthorizationRequirements()
    {
        var list = Items.Select(x => new AuthorizationRequirement(x.Policy)).ToList();

        var allows = Items.SelectMany(x => x.Allows);

        foreach (var allow in allows)
            list.Add(new AuthorizationRequirement(allow));

        return list;
    }

    public RequirementList()
    {
        var reflector = new Shift.Common.Reflector();

        var policies = reflector.FindConstants(typeof(Policies), '.');

        foreach (var policy in policies)
        {
            var requirement = new Requirement(policy.Value);

            var item = Items.Find(x => x.Policy == requirement.Policy);

            if (item == null)
            {
                Items.Add(requirement);
            }
            else
            {
                item.Allows.Add(policy.Value);
            }
        }
    }
}
