using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Shift.Common
{
    /// <summary>
    /// Loads permissions from a JSON file and expands wildcards at load time.
    /// </summary>
    /// <remarks>
    /// Returns ResourcePermissions[] rather than PermissionList because:
    /// - ResourcePermissions is a data transfer format (resource-centric with routes)
    /// - PermissionList is a runtime container with indexes for O(1) lookups
    /// - Callers may need to merge, filter, or transform data before adding to PermissionList
    /// - Use PopulatePermissionList() to convert and add to a PermissionList
    ///
    /// Wildcards are supported for:
    /// - Resource paths: "workflow/*" matches all resources starting with "workflow/"
    /// - Route paths: "api/workflow/*" matches all routes starting with "api/workflow/"
    /// - Role names: "*" matches all known roles
    /// - Access values: "*" expands to all flags for that access type (handled by AccessSet)
    ///
    /// All wildcards are expanded during loading so no pattern-matching is required at runtime.
    /// </remarks>
    public class PermissionListLoader
    {
        private const string Wildcard = "*";

        private readonly HashSet<string> _knownResources;
        private readonly HashSet<string> _knownRoutes;
        private readonly HashSet<string> _knownRoles;
        private readonly Dictionary<string, List<string>> _resourceRouteMap;

        public PermissionListLoader()
            : this(Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>())
        {
        }

        public PermissionListLoader(
            IEnumerable<string> knownResources,
            IEnumerable<string> knownRoles)
            : this(knownResources, knownRoles, Enumerable.Empty<string>())
        {
        }

        public PermissionListLoader(
            IEnumerable<string> knownResources,
            IEnumerable<string> knownRoles,
            IEnumerable<string> knownRoutes)
        {
            _knownResources = new HashSet<string>(knownResources, StringComparer.OrdinalIgnoreCase);
            _knownRoles = new HashSet<string>(knownRoles, StringComparer.OrdinalIgnoreCase);
            _knownRoutes = new HashSet<string>(knownRoutes, StringComparer.OrdinalIgnoreCase);
            _resourceRouteMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registers a resource path that can be matched by wildcards.
        /// </summary>
        public void AddResource(string resource)
        {
            if (!string.IsNullOrEmpty(resource))
                _knownResources.Add(resource);
        }

        /// <summary>
        /// Registers a resource with its associated routes.
        /// </summary>
        public void AddResource(string resource, IEnumerable<string> routes)
        {
            AddResource(resource);

            if (!_resourceRouteMap.TryGetValue(resource, out var routeList))
            {
                routeList = new List<string>();
                _resourceRouteMap[resource] = routeList;
            }

            foreach (var route in routes)
            {
                AddRoute(route);
                if (!routeList.Contains(route, StringComparer.OrdinalIgnoreCase))
                    routeList.Add(route);
            }
        }

        /// <summary>
        /// Registers a resource with its associated routes from a Resource object.
        /// </summary>
        public void AddResource(Resource resource)
        {
            if (resource == null)
                return;

            AddResource(resource.Path, resource.Routes);
        }

        /// <summary>
        /// Registers multiple resource paths that can be matched by wildcards.
        /// </summary>
        public void AddResources(IEnumerable<string> resources)
        {
            foreach (var resource in resources)
                AddResource(resource);
        }

        /// <summary>
        /// Registers multiple resources with their associated routes.
        /// </summary>
        public void AddResources(IEnumerable<Resource> resources)
        {
            foreach (var resource in resources)
                AddResource(resource);
        }

        /// <summary>
        /// Registers a route path that can be matched by wildcards.
        /// </summary>
        public void AddRoute(string route)
        {
            if (!string.IsNullOrEmpty(route))
                _knownRoutes.Add(route);
        }

        /// <summary>
        /// Registers multiple route paths that can be matched by wildcards.
        /// </summary>
        public void AddRoutes(IEnumerable<string> routes)
        {
            foreach (var route in routes)
                AddRoute(route);
        }

        /// <summary>
        /// Registers a role name that can be matched by wildcards.
        /// </summary>
        public void AddRole(string role)
        {
            if (!string.IsNullOrEmpty(role))
                _knownRoles.Add(role);
        }

        /// <summary>
        /// Registers multiple role names that can be matched by wildcards.
        /// </summary>
        public void AddRoles(IEnumerable<string> roles)
        {
            foreach (var role in roles)
                AddRole(role);
        }

        /// <summary>
        /// Loads and expands permissions from a JSON file.
        /// </summary>
        public ResourcePermissions[] LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"Permission file not found: {filePath}", filePath);

            var json = System.IO.File.ReadAllText(filePath);

            return LoadFromJson(json);
        }

        /// <summary>
        /// Loads and expands permissions from a JSON string.
        /// </summary>
        public ResourcePermissions[] LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));

            var rawPermissions = JsonConvert.DeserializeObject<ResourcePermissions[]>(json);

            if (rawPermissions == null || rawPermissions.Length == 0)
                return Array.Empty<ResourcePermissions>();

            return ExpandWildcards(rawPermissions);
        }

        /// <summary>
        /// Loads and expands permissions from a JSON string.
        /// </summary>
        public ResourcePermissions[] LoadFromArray(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));

            var rawPermissions = JsonConvert.DeserializeObject<ResourcePermissions[]>(json);

            if (rawPermissions == null || rawPermissions.Length == 0)
                return Array.Empty<ResourcePermissions>();

            return ExpandWildcards(rawPermissions);
        }

        /// <summary>
        /// Loads permissions from a JSON file and populates a PermissionList.
        /// </summary>
        /// <param name="filePath">Path to the JSON file.</param>
        /// <param name="permissionList">The PermissionList to populate.</param>
        public void PopulateFromFile(string filePath, PermissionList permissionList, AccessOperation operation)
        {
            var permissions = LoadFromFile(filePath);
            Populate(permissions, permissionList, operation);
        }

        /// <summary>
        /// Loads permissions from a JSON string and populates a PermissionList.
        /// </summary>
        /// <param name="json">JSON content.</param>
        /// <param name="permissionList">The PermissionList to populate.</param>
        public void PopulateFromJson(string json, PermissionList permissionList, AccessOperation operation)
        {
            var permissions = LoadFromJson(json);
            Populate(permissions, permissionList, operation);
        }

        /// <summary>
        /// Populates a PermissionList from ResourcePermissions data.
        /// </summary>
        /// <param name="permissions">The permissions data to add.</param>
        /// <param name="permissionList">The PermissionList to populate.</param>
        public void Populate(ResourcePermissions[] permissions, PermissionList permissionList, AccessOperation operation)
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            if (permissionList == null)
                throw new ArgumentNullException(nameof(permissionList));

            foreach (var resourcePermission in permissions)
            {
                // Register resources to the shared registry if available
                permissionList.Resources?.AddResource(resourcePermission.Resource, resourcePermission.Routes);

                // Add permissions for each role
                foreach (var roleAccess in resourcePermission.Permissions)
                {
                    foreach (var role in roleAccess.Roles)
                    {
                        var permission = new Permission(resourcePermission.Resource, role);

                        permission.Access.Add(roleAccess.Access, operation);

                        permissionList.Add(permission);
                    }
                }
            }
        }

        /// <summary>
        /// Expands all wildcards in the permissions.
        /// </summary>
        public ResourcePermissions[] ExpandWildcards(ResourcePermissions[] permissions)
        {
            var expanded = new List<ResourcePermissions>();

            foreach (var permission in permissions)
            {
                var isResourceWildcard = ContainsWildcard(permission.Resource);
                var expandedResources = ExpandResourceWildcard(permission.Resource).ToList();

                foreach (var resource in expandedResources)
                {
                    var expandedRoutes = ExpandRoutes(permission.Routes, resource);

                    var expandedPermission = new ResourcePermissions
                    {
                        Resource = resource,
                        Routes = expandedRoutes,
                        Permissions = ExpandRoleWildcards(permission.Permissions),
                        IsExpanded = isResourceWildcard
                    };

                    expanded.Add(expandedPermission);
                }
            }

            return MergePermissions(expanded);
        }

        /// <summary>
        /// Expands route patterns for a given resource.
        /// </summary>
        private List<string> ExpandRoutes(List<string> routePatterns, string resource)
        {
            var routes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // If no route patterns specified, use routes from resource-route map
            if (routePatterns == null || routePatterns.Count == 0)
            {
                if (_resourceRouteMap.TryGetValue(resource, out var mappedRoutes))
                {
                    foreach (var route in mappedRoutes)
                        routes.Add(route);
                }
                return routes.ToList();
            }

            // Expand each route pattern
            foreach (var pattern in routePatterns)
            {
                foreach (var route in ExpandRouteWildcard(pattern))
                    routes.Add(route);
            }

            return routes.ToList();
        }

        /// <summary>
        /// Expands a route pattern containing wildcards into matching route paths.
        /// </summary>
        private IEnumerable<string> ExpandRouteWildcard(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                yield break;

            if (!ContainsWildcard(pattern))
            {
                yield return pattern;
                yield break;
            }

            if (pattern == Wildcard)
            {
                foreach (var route in _knownRoutes)
                    yield return route;
                yield break;
            }

            var regex = WildcardToRegex(pattern);

            foreach (var route in _knownRoutes)
            {
                if (regex.IsMatch(route))
                    yield return route;
            }
        }

        /// <summary>
        /// Expands a resource pattern containing wildcards into matching resource paths.
        /// </summary>
        private IEnumerable<string> ExpandResourceWildcard(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                yield break;

            if (!ContainsWildcard(pattern))
            {
                yield return pattern;
                yield break;
            }

            if (pattern == Wildcard)
            {
                foreach (var resource in _knownResources)
                    yield return resource;
                yield break;
            }

            var regex = WildcardToRegex(pattern);

            foreach (var resource in _knownResources)
            {
                if (regex.IsMatch(resource))
                    yield return resource;
            }
        }

        /// <summary>
        /// Expands role wildcards in permission bundles.
        /// </summary>
        private List<RoleAccessBundle> ExpandRoleWildcards(List<RoleAccessBundle> bundles)
        {
            var expanded = new List<RoleAccessBundle>();

            foreach (var bundle in bundles)
            {
                var expandedRoles = new List<string>();

                foreach (var role in bundle.Roles)
                {
                    expandedRoles.AddRange(ExpandRoleWildcard(role));
                }

                if (expandedRoles.Count > 0)
                {
                    expanded.Add(new RoleAccessBundle
                    {
                        Roles = expandedRoles.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                        Access = bundle.Access
                    });
                }
            }

            return expanded;
        }

        /// <summary>
        /// Expands a role pattern containing wildcards into matching role names.
        /// </summary>
        private IEnumerable<string> ExpandRoleWildcard(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                yield break;

            if (!ContainsWildcard(pattern))
            {
                yield return pattern;
                yield break;
            }

            if (pattern == Wildcard)
            {
                foreach (var role in _knownRoles)
                    yield return role;
                yield break;
            }

            var regex = WildcardToRegex(pattern);

            foreach (var role in _knownRoles)
            {
                if (regex.IsMatch(role))
                    yield return role;
            }
        }

        /// <summary>
        /// Merges permissions for the same resource into a single entry.
        /// </summary>
        private ResourcePermissions[] MergePermissions(List<ResourcePermissions> permissions)
        {
            var grouped = permissions
                .GroupBy(p => p.Resource, StringComparer.OrdinalIgnoreCase)
                .Select(g => new ResourcePermissions
                {
                    Resource = g.Key,
                    Routes = g.SelectMany(p => p.Routes).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                    Permissions = MergeRoleAccessBundles(g.SelectMany(p => p.Permissions).ToList()),
                    IsExpanded = g.Any(p => p.IsExpanded)
                })
                .ToArray();

            return grouped;
        }

        /// <summary>
        /// Merges role access bundles, combining access for duplicate role sets.
        /// </summary>
        private List<RoleAccessBundle> MergeRoleAccessBundles(List<RoleAccessBundle> bundles)
        {
            var merged = new Dictionary<string, RoleAccessBundle>(StringComparer.OrdinalIgnoreCase);

            foreach (var bundle in bundles)
            {
                var key = string.Join("\0", bundle.Roles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase));

                if (merged.TryGetValue(key, out var existing))
                {
                    existing.Access.Add(bundle.Access);
                }
                else
                {
                    merged[key] = new RoleAccessBundle
                    {
                        Roles = bundle.Roles.ToList(),
                        Access = CloneAccessSet(bundle.Access)
                    };
                }
            }

            return merged.Values.ToList();
        }

        private AccessSet CloneAccessSet(AccessSet source)
        {
            var clone = new AccessSet
            {
                Feature = source.Feature,
                Data = source.Data,
                Authority = source.Authority
            };
            return clone;
        }

        private static bool ContainsWildcard(string pattern)
        {
            return pattern.Contains(Wildcard);
        }

        /// <summary>
        /// Converts a wildcard pattern to a regex for matching.
        /// Supports * (match any characters) and ? (match single character).
        /// </summary>
        private static Regex WildcardToRegex(string pattern)
        {
            var escaped = Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".");

            return new Regex($"^{escaped}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
