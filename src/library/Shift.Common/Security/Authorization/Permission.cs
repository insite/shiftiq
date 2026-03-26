using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class Permission
    {
        public Resource Resource { get; set; }
        public Role Role { get; set; }
        public AccessControl Access { get; set; }

        public Permission() : this(new Resource(string.Empty), new Role(string.Empty)) { }

        public Permission(string resource, string role) : this(new Resource(resource), new Role(role)) { }

        public Permission(Resource resource, Role role)
        {
            Resource = resource;
            Role = role;
            Access = new AccessControl();
        }
    }

    /// <summary>
    /// Shared registry for resources and route mappings. Resources are global to the application
    /// and don't vary by organization, so they're stored once and shared across all PermissionLists.
    /// </summary>
    public class ResourceRegistry : IEnumerable<KeyValuePair<string, Resource>>
    {
        private readonly List<Resource> _resources = new List<Resource>();
        private readonly Dictionary<string, Resource> _resourcesByName = new Dictionary<string, Resource>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, HashSet<Resource>> _resourcesByRoute = new Dictionary<string, HashSet<Resource>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _subroutesByRoute = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<Resource> Resources => _resources;
        public IReadOnlyDictionary<string, Resource> ResourcesByName => _resourcesByName;
        public int Count => _resources.Count;

        public Resource AddResource(string name)
        {
            if (_resourcesByName.TryGetValue(name, out var resource))
                return resource;

            resource = new Resource(name);
            _resources.Add(resource);
            _resourcesByName[name] = resource;

            return resource;
        }

        public void AddResource(string name, List<string> routes)
        {
            var resource = AddResource(name);
            resource.AddRoutes(routes);
            IndexResourceRoutes(resource);
        }

        public void AddResource(Resource resource)
        {
            if (_resourcesByName.TryGetValue(resource.Path, out var existing))
            {
                existing.AddRoutes(resource.Routes);
                IndexResourceRoutes(existing);
                return;
            }

            var item = new Resource(resource.Path);
            item.Routes.AddRange(resource.Routes);

            _resources.Add(item);
            _resourcesByName[item.Path] = item;
            IndexResourceRoutes(item);
        }

        public void AddResources(IEnumerable<Resource> resources)
        {
            foreach (var resource in resources)
                AddResource(resource);
        }

        private void IndexResourceRoutes(Resource resource)
        {
            foreach (var route in resource.Routes)
            {
                if (string.IsNullOrEmpty(route))
                    continue;

                if (!_resourcesByRoute.TryGetValue(route, out var resources))
                {
                    resources = new HashSet<Resource>();
                    _resourcesByRoute[route] = resources;
                }
                resources.Add(resource);
            }
        }

        public bool ContainsResource(string resource) => _resourcesByName.ContainsKey(resource);

        public bool ContainsRoute(string route) => _resourcesByRoute.ContainsKey(route);

        public Resource FindResource(string resource)
            => _resourcesByName.TryGetValue(resource, out var r) ? r : null;

        public IEnumerable<Resource> FindResourcesByRoute(string route)
            => _resourcesByRoute.TryGetValue(route, out var resources)
                ? resources
                : Enumerable.Empty<Resource>();

        public void Clear()
        {
            _resources.Clear();
            _resourcesByName.Clear();
            _resourcesByRoute.Clear();
            _subroutesByRoute.Clear();
        }

        /// <summary>
        /// Sets the expanded subroutes (child route patterns) for a route.
        /// </summary>
        public void SetSubroutes(string route, List<string> subroutes)
        {
            if (string.IsNullOrEmpty(route) || subroutes == null)
                return;

            _subroutesByRoute[route] = subroutes;
        }

        /// <summary>
        /// Gets the expanded subroutes (child route patterns) for a route.
        /// Returns an empty list if no subroutes are defined.
        /// </summary>
        public List<string> GetSubroutes(string route)
        {
            if (string.IsNullOrEmpty(route))
                return new List<string>();

            return _subroutesByRoute.TryGetValue(route, out var subroutes)
                ? subroutes
                : new List<string>();
        }

        /// <summary>
        /// Checks if a resource exists by key (alias for ContainsResource).
        /// </summary>
        public bool ContainsKey(string resource) => ContainsResource(resource);

        public IEnumerator<KeyValuePair<string, Resource>> GetEnumerator()
            => _resourcesByName.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerable<string> GetResourceNames()
        {
            return _resourcesByName.Keys.OrderBy(x => x);
        }

        public IEnumerable<string> GetRouteUrls()
        {
            return _resourcesByRoute.Keys.OrderBy(x => x);
        }
    }

    public class PermissionList
    {
        private readonly List<Permission> _permissions = new List<Permission>();
        private readonly List<Role> _roles = new List<Role>();
        private readonly Dictionary<string, Role> _rolesByName = new Dictionary<string, Role>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, Role> _rolesById = new Dictionary<Guid, Role>();
        private readonly Dictionary<string, Permission> _permissionIndex = new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Shared resource registry. When null, route-based lookups are not available.
        /// </summary>
        public ResourceRegistry Resources { get; set; }

        public List<Permission> Items => _permissions;

        public void Add(Permission item)
        {
            var key = GetPermissionKey(item.Resource.Path, item.Role.Name);

            if (_permissionIndex.TryGetValue(key, out var existing))
            {
                existing.Access.Add(item.Access);
                return;
            }

            _permissions.Add(item);
            _permissionIndex[key] = item;

            AddRole(item.Role);
        }

        public void Add(string resource, string role, string access)
        {
            var permission = new Permission(resource, role);

            permission.Access.Add(access);

            Add(permission);
        }

        private static string GetPermissionKey(string resource, string role) => $"{resource}\0{role}";

        public Role AddRole(string name)
        {
            if (_rolesByName.TryGetValue(name, out var role))
                return role;

            role = new Role(name);
            _roles.Add(role);
            _rolesByName[name] = role;
            _rolesById[role.Identifier] = role;

            return role;
        }

        public void AddRole(Role role)
        {
            if (_rolesByName.ContainsKey(role.Name))
                return;

            var item = new Role(role.Name, role.Identifier);
            _roles.Add(item);
            _rolesByName[item.Name] = item;
            _rolesById[item.Identifier] = item;
        }

        public void Clear()
        {
            _permissions.Clear();
            _roles.Clear();

            _rolesByName.Clear();
            _rolesById.Clear();

            _permissionIndex.Clear();
        }

        public bool ContainsResource(string resource) => Resources?.ContainsResource(resource) ?? false;

        public bool ContainsRole(string role) => _rolesByName.ContainsKey(role);

        public bool ContainsRoute(string route) => Resources?.ContainsRoute(route) ?? false;

        /// <summary>
        /// Finds all resources that have the specified route.
        /// </summary>
        public IEnumerable<Resource> FindResourcesByRoute(string route)
            => Resources?.FindResourcesByRoute(route) ?? Enumerable.Empty<Resource>();

        #region Route-based permission checks

        /// <summary>
        /// Checks if access is allowed for any resource containing the specified route.
        /// Returns true if ANY matching resource grants access (union semantics).
        /// </summary>
        public bool IsAllowedByRoute(string route, string role)
            => FindResourcesByRoute(route)
                .Any(r => FindPermission(r.Path, role)?.Access.IsAllowed() ?? false);

        public bool IsAllowedByRoute(string route, IEnumerable<string> roles)
            => FindResourcesByRoute(route)
                .Any(r => roles.Any(role => FindPermission(r.Path, role)?.Access.IsAllowed() ?? false));

        public bool IsAllowedByRoute(string route, IEnumerable<Role> roles)
            => IsAllowedByRoute(route, roles.Select(r => r.Name));

        public bool IsAllowedByRoute(string route, string role, FeatureAccess access)
            => FindResourcesByRoute(route)
                .Any(r => FindPermission(r.Path, role)?.Access.IsAllowed(access) ?? false);

        public bool IsAllowedByRoute(string route, IEnumerable<string> roles, FeatureAccess access)
            => FindResourcesByRoute(route)
                .Any(r => roles.Any(role => FindPermission(r.Path, role)?.Access.IsAllowed(access) ?? false));

        public bool IsAllowedByRoute(string route, string role, DataAccess access)
        {
            var resources = FindResourcesByRoute(route);

            foreach (var resource in resources)
            {
                var permission = FindPermission(resource.Path, role);

                if (permission == null)
                    continue;

                if (permission.Access.IsAllowed(access))
                    return true;
            }

            return false;
        }

        public bool IsAllowedByRoute(string route, IEnumerable<string> roles, DataAccess access)
            => FindResourcesByRoute(route)
                .Any(r => roles.Any(role => FindPermission(r.Path, role)?.Access.IsAllowed(access) ?? false));

        public bool IsAllowedByRoute(string route, string role, AuthorityAccess access)
            => FindResourcesByRoute(route)
                .Any(r => FindPermission(r.Path, role)?.Access.IsAllowed(access) ?? false);

        public bool IsAllowedByRoute(string route, IEnumerable<string> roles, AuthorityAccess access)
            => FindResourcesByRoute(route)
                .Any(r => roles.Any(role => FindPermission(r.Path, role)?.Access.IsAllowed(access) ?? false));

        public bool IsDeniedByRoute(string route, string role)
            => FindResourcesByRoute(route)
                .Any(r => FindPermission(r.Path, role)?.Access.IsDenied() ?? false);

        public bool IsDeniedByRoute(string route, IEnumerable<string> roles)
            => FindResourcesByRoute(route)
                .Any(r => roles.Any(role => FindPermission(r.Path, role)?.Access.IsDenied() ?? false));

        #endregion Route-based permission checks

        public bool IsAllowed(string resource, string role)
            => FindPermission(resource, role)?.Access.IsAllowed() ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles)
            => roles.Any(role => IsAllowed(resource, role));

        public bool IsAllowed(string resource, IEnumerable<Role> roles)
            => roles.Any(role => IsAllowed(resource, role.Name));

        public bool IsAllowed(string resource, string role, FeatureAccess access)
            => FindPermission(resource, role)?.Access.IsAllowed(access) ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles, FeatureAccess access)
            => roles.Any(role => IsAllowed(resource, role, access));

        public bool IsAllowed(string resource, string role, DataAccess access)
            => FindPermission(resource, role)?.Access.IsAllowed(access) ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles, DataAccess access)
            => roles.Any(role => IsAllowed(resource, role, access));

        public bool IsAllowed(string resource, string role, AuthorityAccess access)
            => FindPermission(resource, role)?.Access.IsAllowed(access) ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles, AuthorityAccess access)
            => roles.Any(role => IsAllowed(resource, role, access));

        public bool IsDenied(string resource, IEnumerable<string> roles)
            => roles.Any(role => IsDenied(resource, role));

        public bool IsDenied(string resource, string role)
            => FindPermission(resource, role)?.Access.IsDenied() ?? false;

        public void Reindex()
        {
            _roles.Clear();
            _rolesByName.Clear();
            _rolesById.Clear();
            _permissionIndex.Clear();

            foreach (var permission in _permissions)
            {
                AddRole(permission.Role);

                var key = GetPermissionKey(permission.Resource.Path, permission.Role.Name);
                _permissionIndex[key] = permission;
            }
        }

        public Permission FindPermission(string resource, string role)
        {
            var key = GetPermissionKey(resource, role);

            if (_permissionIndex.TryGetValue(key, out var p))
                return p;

            return null;
        }

        public Resource FindResource(string resource)
            => Resources?.FindResource(resource);

        public Role FindRole(string role)
            => _rolesByName.TryGetValue(role, out var r) ? r : null;

        public Role FindRole(Guid role)
            => _rolesById.TryGetValue(role, out var r) ? r : null;
    }

    /// <summary>
    /// The set of all permission lists, indexed by organization (tenant) account code.
    /// </summary>
    /// <remarks>
    /// Each organization has its own permission list. This helps to ensure permissions granted in one organization
    /// account never leak into another account. Also this makes it easy to implement a partition-wide permission 
    /// matrix.
    /// </remarks>
    public class PermissionMatrix
    {
        /// <summary>
        /// The collection of all permissions for all organizations.
        /// </summary>
        public Dictionary<string, PermissionList> Permissions { get; set; } = new Dictionary<string, PermissionList>();

        /// <summary>
        /// Shared registry for all resources. Resources are global across all organizations.
        /// </summary>
        public ResourceRegistry Resources { get; set; } = new ResourceRegistry();

        public string[] Organizations => Permissions.Keys.OrderBy(x => x).ToArray();

        public PermissionList GetPermissions(string organization)
        {
            if (string.IsNullOrEmpty(organization))
                return new PermissionList { Resources = Resources };

            if (Permissions.TryGetValue(organization, out var list))
                return list;

            return new PermissionList { Resources = Resources };
        }

        public bool TryGetPermissions(string organization, out PermissionList list)
        {
            return Permissions.TryGetValue(organization, out list);
        }

        public PermissionList GetOrCreatePermissions(string organization)
        {
            if (!Permissions.TryGetValue(organization, out var list))
            {
                list = new PermissionList { Resources = Resources };
                Permissions[organization] = list;
            }
            return list;
        }

        public void AddPermissions(string organization, PermissionList list)
        {
            if (Permissions.ContainsKey(organization))
                throw new InvalidOperationException($"The permission matrix already contains a list for this organization: {organization}");

            list.Resources = Resources;
            Permissions[organization] = list;
        }

        public void AddPermissions(AccessOperation operation, ResourcePermissions[] resourcePermissions, string organization)
        {
            foreach (var resourcePermission in resourcePermissions)
                AddPermissions(operation, resourcePermission, organization);
        }

        public void AddPermissions(AccessOperation operation, ResourcePermissions resourcePermissions, string organization)
        {
            var resource = resourcePermissions.Resource;

            AddResource(resourcePermissions.Resource);

            var list = GetOrCreatePermissions(organization);

            foreach (var roleAccessBundle in resourcePermissions.Permissions)
            {
                foreach (var role in roleAccessBundle.Roles)
                {
                    var permission = list.FindPermission(resource, role);

                    if (permission == null)
                    {
                        permission = new Permission(resource, role);

                        list.Add(permission);
                    }

                    if (operation == AccessOperation.Grant)
                    {
                        if (!roleAccessBundle.Access.HasAny())
                        {
                            permission.Access.Grant(FeatureAccess.Trial);
                            permission.Access.Grant(FeatureAccess.Use);
                        }

                        permission.Access.Granted.Add(roleAccessBundle.Access);
                    }

                    if (operation == AccessOperation.Deny)
                    {
                        if (!roleAccessBundle.Access.HasAny())
                        {
                            permission.Access.Deny(FeatureAccess.Trial);
                            permission.Access.Deny(FeatureAccess.Use);
                        }

                        permission.Access.Denied.Add(roleAccessBundle.Access);
                    }
                }
            }
        }

        public void AddResource(string resource)
        {
            Resources.AddResource(resource);
        }

        public bool IsAllowed(string organization, string resource, string role, FeatureAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, string role, DataAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, string role, AuthorityAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, string[] roles, DataAccess? access)
        {
            if (!TryGetPermissions(organization, out var list))
                return false;

            if (access == null)
            {
                if (roles.Any(role => list.IsAllowed(resource, role)))
                    return true;

                if (roles.Any(role => list.IsAllowedByRoute(resource, role)))
                    return true;
            }
            else
            {
                if (roles.Any(role => list.IsAllowed(resource, role, access.Value)))
                    return true;

                if (roles.Any(role => list.IsAllowedByRoute(resource, role, access.Value)))
                    return true;
            }

            return false;
        }

        public bool IsDenied(string organization, string resource, string role, FeatureAccess access)
            => TryGetPermissions(organization, out var list) && list.IsDenied(resource, role);

        public void MergePermissions(string partition)
        {
            if (!Permissions.ContainsKey(partition))
                throw new InvalidOperationException($"The permission matrix does not contain an organization identified by the partition {partition}");

            var partitionPermissions = Permissions[partition];

            var organizations = Organizations.Where(x => x != partition).OrderBy(x => x).ToList();

            foreach (var organization in organizations)
            {
                var organizationPermissions = Permissions[organization];

                foreach (var partitionPermissionItem in partitionPermissions.Items)
                {
                    organizationPermissions.Add(partitionPermissionItem);
                }
            }
        }

        public bool ContainsOrganization(string partition)
        {
            return Organizations.Any(organization => StringHelper.Equals(organization, partition));
        }
    }

    public static class PermissionContext
    {
        public static Func<PermissionMatrix> GetMatrix { get; set; }
    }
}