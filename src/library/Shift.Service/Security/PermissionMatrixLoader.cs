using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class PermissionMatrixLoader
{
    private const string Wildcard = "*";

    private readonly ReleaseSettings _releaseSettings;
    private readonly SecuritySettings _securitySettings;
    private readonly OrganizationService _organizationService;
    private readonly IActionService _actionService;
    private readonly PermissionService _permissionService;
    private readonly QueryTypeCollection _queryTypes;

    public PermissionMatrixLoader(
        ReleaseSettings releaseSettings,
        SecuritySettings securitySettings,
        OrganizationService organizationService,
        IActionService actionService,
        PermissionService permissionService,
        QueryTypeCollection queryTypes)
    {
        _releaseSettings = releaseSettings;
        _securitySettings = securitySettings;
        _organizationService = organizationService;
        _actionService = actionService;
        _permissionService = permissionService;
        _queryTypes = queryTypes;
    }

    public async Task LoadAsync(PermissionMatrix matrix, CancellationToken cancellation = default)
    {
        var queryResources = _queryTypes.GetResources();

        var actionsTask = _actionService.DownloadAsync(new CollectActions(), cancellation);

        var permissionsTask = _permissionService.DownloadAsync(new CollectPermissions(), cancellation);

        var organizationsTask = MaterializeAsync(_organizationService.DownloadAsync(new CollectOrganizations(), cancellation), cancellation);

        await Task.WhenAll(actionsTask, permissionsTask, organizationsTask);

        var actions = actionsTask.Result;

        var permissions = permissionsTask.Result;

        var organizations = organizationsTask.Result;

        var actionsByIdentifier = actions.ToDictionary(x => x.ActionIdentifier);

        var permissionsByOrg = permissions
            .GroupBy(x => x.OrganizationCode)
            .ToDictionary(g => g.Key, g => g.ToList());

        var inferences = ActionHelper.GetIndirectRelations(actions);

        var inferencesByParent = inferences
            .GroupBy(x => x.ParentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var permissionsByObject = permissions
            .GroupBy(x => x.ObjectIdentifier)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var organization in organizations)
        {
            if (organization.AccountClosed.HasValue)
                continue;

            var code = organization.OrganizationCode;

            var list = new PermissionList();

            list.AddResources(queryResources);

            foreach (var action in actions)
                list.AddResource(new Resource(action.ActionUrl, action.ActionIdentifier));

            if (!permissionsByOrg.TryGetValue(code, out var orgPermissions))
                orgPermissions = new List<PermissionModel>();

            var roleCache = new Dictionary<Guid, Role>();

            foreach (var permission in orgPermissions)
            {
                var resource = list.FindResource(permission.ObjectIdentifier);

                if (resource == null && actionsByIdentifier.TryGetValue(permission.ObjectIdentifier, out var action))
                {
                    resource = new Resource(action.ActionUrl, permission.ObjectIdentifier);

                    list.AddResource(resource);
                }

                if (resource == null)
                    continue;

                var role = GetOrCreateRole(list, roleCache, permission.GroupIdentifier, permission.GroupName);

                var item = new Permission(resource, role);

                item.Access.Granted.Switch = SwitchAccess.On;

                list.Add(item);
            }

            foreach (var inference in inferences)
            {
                var childResource = list.FindResource(inference.ChildId);

                if (childResource == null)
                    continue;

                if (!permissionsByObject.TryGetValue(inference.ParentId, out var parentPermissions))
                    continue;

                foreach (var parentPermission in parentPermissions)
                {
                    var role = GetOrCreateRole(list, roleCache, parentPermission.GroupIdentifier, parentPermission.GroupName);

                    var item = new Permission(childResource, role);

                    item.Access.Granted.Switch = SwitchAccess.On;

                    list.Add(item);
                }
            }

            // Explicitly grant full access on all resources to all system-level operators.

            // TODO: Instead of having this access control rule hardcoded, read the list of platform-wide access control
            // rules from config/security/access-granted.json. This will allow permission changes without code changes.

            list.Add(Wildcard, SystemRole.Operator, "allow:" + Wildcard);

            matrix.AddPermissions(code, list);
        }
    }

    private static Role GetOrCreateRole(PermissionList list, Dictionary<Guid, Role> cache, Guid id, string name)
    {
        if (cache.TryGetValue(id, out var role))
            return role;

        role = list.FindRole(id) ?? new Role(name, id);

        list.AddRole(role);

        cache[id] = role;

        return role;
    }

    private static async Task<List<T>> MaterializeAsync<T>(IAsyncEnumerable<T> source, CancellationToken cancellation = default)
    {
        var list = new List<T>();

        await foreach (var item in source.WithCancellation(cancellation).ConfigureAwait(false))
            list.Add(item);

        return list;
    }
}

public class InferredRelation
{
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }
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
                ParentId = ancestor,
                ChildId = child
            });

            Traverse(ancestor, child, children, relations);
        }
    }
}