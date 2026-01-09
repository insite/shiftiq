using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class PermissionBundle
    {
        public List<string> Resources { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Access { get; set; }

        public PermissionBundle()
        {
            Resources = new List<string>();
            Roles = new List<string>();
            Access = new List<string>();
        }

        /// <summary>
        /// Returns a list of discrete permissions.
        /// </summary>
        /// <remarks>
        /// An individual permission is a list of access rules assigned to a specific role on a specific resource. A
        /// permission bundle represents a compressed aggregation, where a list of access rules is assigned to multiple
        /// roles on multiple resources. This is a useful shorthand for file-based configuration settings.
        /// 
        /// Note: If a bundle has no access rules explicitly defined, then Basic Allow is assumed by default. This 
        /// allows simple configuration settings like "grant access to roles A and B on resources X and Y".
        /// </remarks>
        public List<Permission> Flatten()
        {
            var list = new List<Permission>();

            if (!Resources.Any())
                throw new InvalidOperationException("Permission bundle requires at least one resource");

            if (!Roles.Any())
                throw new InvalidOperationException("Permission bundle requires at least one role");

            foreach (var resource in Resources)
            {
                foreach (var role in Roles)
                {
                    var item = CreatePermission(resource, role);

                    list.Add(item);
                }
            }

            return list;
        }

        private Permission CreatePermission(string resource, string role)
        {
            var permission = new Permission(resource, role);

            if (Access.Any())
                foreach (var access in Access)
                    permission.Access.Add(access);
            else
                permission.Access.Grant(SwitchAccess.On);

            return permission;
        }

        public void Add(Permission permission)
        {
            Resources.Add(permission.Resource.Name);
            Roles.Add(permission.Role.Name);
        }
    }

    public class PermissionList
    {
        private const string Wildcard = "*";

        private readonly List<Permission> _permissions = new List<Permission>();
        private readonly List<Resource> _resources = new List<Resource>();
        private readonly List<Role> _roles = new List<Role>();

        // Indexes for O(1) lookups
        private readonly Dictionary<string, Resource> _resourcesByName = new Dictionary<string, Resource>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, Resource> _resourcesById = new Dictionary<Guid, Resource>();
        private readonly Dictionary<string, Role> _rolesByName = new Dictionary<string, Role>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, Role> _rolesById = new Dictionary<Guid, Role>();
        private readonly Dictionary<string, Permission> _permissionIndex = new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);

        public List<Permission> Items => _permissions;

        public void Add(PermissionBundle bundle)
        {
            var list = bundle.Flatten();
            foreach (var item in list)
                Add(item);
        }

        public void Add(Permission item)
        {
            var key = GetPermissionKey(item.Resource.Name, item.Role.Name);

            if (_permissionIndex.TryGetValue(key, out var existing))
            {
                existing.Access.Add(item.Access);
                return;
            }

            _permissions.Add(item);
            _permissionIndex[key] = item;

            AddResource(item.Resource);
            AddRole(item.Role);
        }

        public void Add(string resource, string role, string access)
        {
            var permission = new Permission(resource, role);

            permission.Access.Add(access);

            Add(permission);
        }

        private static string GetPermissionKey(string resource, string role) => $"{resource}\0{role}";

        public Resource AddResource(string name)
        {
            if (_resourcesByName.TryGetValue(name, out var resource))
                return resource;

            resource = new Resource(name);
            _resources.Add(resource);
            _resourcesByName[name] = resource;
            _resourcesById[resource.Identifier] = resource;

            return resource;
        }

        public void AddResource(string name, List<string> aliases)
        {
            var resource = AddResource(name);
            resource.AddAliases(aliases);
        }

        public void AddResources(List<string> resources)
        {
            foreach (var resource in resources)
                AddResource(resource);
        }

        public void AddResource(Resource resource)
        {
            if (_resourcesByName.TryGetValue(resource.Name, out var existing))
            {
                existing.AddAliases(resource.Aliases);
                return;
            }

            var item = new Resource(resource.Name, resource.Identifier);
            item.Aliases.AddRange(resource.Aliases);

            _resources.Add(item);
            _resourcesByName[item.Name] = item;
            _resourcesById[item.Identifier] = item;
        }

        public void AddResources(List<Resource> resources)
        {
            foreach (var resource in resources)
                AddResource(resource);
        }

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

        public bool ContainsResource(string resource) => _resourcesByName.ContainsKey(resource);

        public bool ContainsRole(string role) => _rolesByName.ContainsKey(role);

        public bool IsAllowed(string resource, string role)
            => FindPermission(resource, role)?.Access.IsAllowed() ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles)
            => roles.Any(role => IsAllowed(resource, role));

        public bool IsAllowed(string resource, IEnumerable<Role> roles)
            => roles.Any(role => IsAllowed(resource, role.Name));

        public bool IsAllowed(Guid resource, string role)
            => FindPermission(resource, role)?.Access.IsAllowed() ?? false;

        public bool IsAllowed(Guid resource, IEnumerable<Role> roles)
            => roles.Any(role => IsAllowed(resource, role.Name));

        public bool IsAllowed(string resource, string role, SwitchAccess access)
            => FindPermission(resource, role)?.Access.IsAllowed(access) ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles, SwitchAccess access)
            => roles.Any(role => IsAllowed(resource, role, access));

        public bool IsAllowed(string resource, string role, OperationAccess access)
            => FindPermission(resource, role)?.Access.IsAllowed(access) ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles, OperationAccess access)
            => roles.Any(role => IsAllowed(resource, role, access));

        public bool IsAllowed(string resource, string role, HttpAccess access)
            => FindPermission(resource, role)?.Access.IsAllowed(access) ?? false;

        public bool IsAllowed(string resource, IEnumerable<string> roles, HttpAccess access)
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
            _resources.Clear();
            _resourcesByName.Clear();
            _resourcesById.Clear();
            _roles.Clear();
            _rolesByName.Clear();
            _rolesById.Clear();
            _permissionIndex.Clear();

            foreach (var permission in _permissions)
            {
                AddResource(permission.Resource);
                AddRole(permission.Role);

                var key = GetPermissionKey(permission.Resource.Name, permission.Role.Name);
                _permissionIndex[key] = permission;
            }
        }

        public Permission FindPermission(string resource, string role)
        {
            var wildcardPermissionKey = GetPermissionKey(Wildcard, role);

            var explicitPermissionKey = GetPermissionKey(resource, role);

            return _permissionIndex.TryGetValue(wildcardPermissionKey, out var p)
                ? p
                : _permissionIndex.TryGetValue(explicitPermissionKey, out var q)
                    ? q
                    : null
                    ;
        }

        public Permission FindPermission(Guid resource, string role)
        {
            if (!_resourcesById.TryGetValue(resource, out var r))
                return null;
            return FindPermission(r.Name, role);
        }

        public Resource FindResource(string resource)
            => _resourcesByName.TryGetValue(resource, out var r) ? r : null;

        public Resource FindResource(Guid resource)
            => _resourcesById.TryGetValue(resource, out var r) ? r : null;

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
        /// The collection of all available resources throughout the platform.
        /// </summary>
        public Dictionary<string, Resource> Resources { get; set; } = new Dictionary<string, Resource>();

        public string[] Organizations => Permissions.Keys.OrderBy(x => x).ToArray();

        public PermissionList GetPermissions(string organization)
        {
            if (string.IsNullOrEmpty(organization))
                return new PermissionList();

            if (Permissions.TryGetValue(organization, out var list))
                return list;

            throw new ArgumentOutOfRangeException(nameof(organization), $"Permission list not found for {organization}");
        }

        public bool TryGetPermissions(string organization, out PermissionList list)
        {
            return Permissions.TryGetValue(organization, out list);
        }

        public PermissionList GetOrCreatePermissions(string organization)
        {
            if (!Permissions.TryGetValue(organization, out var list))
            {
                list = new PermissionList();
                Permissions[organization] = list;
            }
            return list;
        }

        public void AddPermissions(string organization, PermissionList list)
        {
            if (Permissions.ContainsKey(organization))
                throw new InvalidOperationException($"The permission matrix already contains a list for this organization: {organization}");

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
                        if (!roleAccessBundle.Access.Any())
                            permission.Access.Grant(SwitchAccess.On);

                        foreach (var access in roleAccessBundle.Access)
                            permission.Access.Granted.Add(access);
                    }

                    if (operation == AccessOperation.Deny)
                    {
                        if (!roleAccessBundle.Access.Any())
                            permission.Access.Deny(SwitchAccess.On);

                        foreach (var access in roleAccessBundle.Access)
                            permission.Access.Denied.Add(access);
                    }
                }
            }
        }

        public void AddResource(string resource)
        {
            if (!Resources.ContainsKey(resource))
                Resources[resource] = new Resource(resource);
        }

        public bool IsAllowed(string organization, string resource, string role, SwitchAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, string role, OperationAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, string role, HttpAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, string role, AuthorityAccess access)
            => TryGetPermissions(organization, out var list) && list.IsAllowed(resource, role, access);

        public bool IsAllowed(string organization, string resource, List<Role> roles)
            => TryGetPermissions(organization, out var list) && roles.Any(role => list.IsAllowed(resource, role.Name));
    }

    public class PermissionMatrixProvider
    {
        private PermissionMatrix _matrix;

        private readonly TaskCompletionSource<bool> _initialized = new TaskCompletionSource<bool>();

        public void SetMatrix(PermissionMatrix matrix)
        {
            _matrix = matrix;
            _initialized.TrySetResult(true);
        }

        public PermissionMatrix Matrix =>
            _matrix ?? throw new InvalidOperationException("PermissionMatrix not yet initialized");

        public Task WaitForInitializationAsync() => _initialized.Task;
    }
}