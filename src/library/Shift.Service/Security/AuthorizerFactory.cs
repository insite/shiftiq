using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class AuthorizerFactory
{
    private readonly SecuritySettings _securitySettings;
    private readonly IActionService _actionService;
    private readonly PermissionService _permissionService;
    private readonly QueryTypeCollection _queryTypes;

    public AuthorizerFactory(SecuritySettings securitySettings, IActionService actionService, PermissionService permissionService, QueryTypeCollection queryTypes)
    {
        _securitySettings = securitySettings;
        _actionService = actionService;
        _permissionService = permissionService;
        _queryTypes = queryTypes;
    }

    public async Task<Authorizer> CreateAsync(CancellationToken cancellation = default)
    {
        var authorizer = new Authorizer(_securitySettings.Domain, _actionService);

        var permissions = await _permissionService.DownloadAsync(new CollectPermissions(), cancellation);

        foreach (var permission in permissions)
        {
            authorizer.Add(
                new Resource { Identifier = permission.ObjectIdentifier },
                new Role { Identifier = permission.GroupIdentifier, Slug = permission.RoleSlug });

            authorizer.Grant(
                permission.ObjectIdentifier,
                permission.GroupIdentifier,
                BasicAccess.Allow);
        }

        if (_securitySettings.Permissions != null)
        {
            foreach (var bundle in _securitySettings.Permissions)
            {
                authorizer.Add(bundle);
            }
        }

        var actions = await _actionService.DownloadAsync(new CollectActions());

        var relationships = ActionHelper.GetIndirectRelations(actions);

        foreach (var relationship in relationships)
        {
            var parentPermissions = permissions.Where(x => x.ObjectIdentifier == relationship.Parent);

            foreach (var parentPermission in parentPermissions)
            {
                authorizer.Add(
                    new Resource { Identifier = relationship.Child },
                    new Role { Identifier = parentPermission.GroupIdentifier });

                authorizer.Grant(
                    relationship.Child,
                    parentPermission.GroupIdentifier,
                    BasicAccess.Allow);
            }
        }

        var queryResources = _queryTypes.GetResources();

        authorizer.AddResources(queryResources);

        return authorizer;
    }
}

public class InferredRelation
{
    public Guid Parent { get; set; }
    public Guid Child { get; set; }
}

public class ActionHelper
{
    public static List<InferredRelation> GetIndirectRelations(IEnumerable<ActionModel> actions)
    {
        var relations = new List<InferredRelation>();

        Dictionary<Guid, List<Guid>> children = actions
            .GroupBy(r => r.PermissionParentActionIdentifier)
            .ToDictionary(g => g.Key ?? Guid.Empty, g => g.Select(r => r.ActionIdentifier).ToList());

        foreach (var action in actions)
        {
            Traverse(action.ActionIdentifier, action.ActionIdentifier, children, relations);
        }

        return relations;
    }

    private static void Traverse(Guid ancestor, Guid current, Dictionary<Guid, List<Guid>> children, List<InferredRelation> relations)
    {
        if (!children.ContainsKey(current))
            return;

        foreach (var child in children[current])
        {
            relations.Add(new InferredRelation
            {
                Parent = ancestor,
                Child = child
            });

            Traverse(ancestor, child, children, relations);
        }
    }
}