using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    /// <summary>
    /// Abstract base class for loading permission matrices from persistent storage.
    /// Inherit from this class and implement the data access methods for your target framework.
    /// </summary>
    public abstract class PermissionMatrixLoaderBase
    {
        private const string Wildcard = "*";

        protected PermissionMatrixLoaderBase()
        {
        }

        #region Abstract data access methods

        protected abstract IReadOnlyList<RouteEndpoint> GetRoutes();
        protected abstract IReadOnlyList<OrganizationInfo> GetActiveOrganizations();
        protected abstract IReadOnlyList<(Guid OrganizationId, GroupPermissionInfo Permission)> GetPermissionsGrantedOnActions();
        protected abstract IReadOnlyList<ResourcePermissions> GetExtraPermissionsGranted(Guid organizationId);
        protected abstract IReadOnlyList<ResourcePermissions> GetExtraPermissionsDenied(Guid organizationId);

        /// <summary>
        /// Gets the subroute configuration. Override to load from a file or other source.
        /// Returns null if no subroute configuration is available.
        /// </summary>
        protected virtual RouteSettings GetRouteSettings()
        {
            return new RouteSettings();
        }

        #endregion

        #region Core loading logic

        public void Load(PermissionMatrix matrix)
        {
            var context = BuildLoadContext();

            matrix.Resources.AddResources(context.Resources); // Add resources to the shared registry (idempotent)

            // Register routes from RouteSettings in the shared registry
            RegisterRoutesFromSettings(matrix.Resources, context.RouteSettings);

            // Expand and store subroutes for each route
            ExpandSubroutes(matrix.Resources, context);

            foreach (var organization in context.Organizations)
            {
                LoadOrganization(matrix, organization, context);
            }
        }

        /// <summary>
        /// Registers explicit routes from RouteSettings in the resource registry.
        /// This ensures routes defined in configuration appear in GetRouteUrls() even if not in the database.
        /// </summary>
        private void RegisterRoutesFromSettings(ResourceRegistry registry, RouteSettings settings)
        {
            if (settings?.Subroutes == null)
                return;

            foreach (var subroute in settings.Subroutes)
            {
                RegisterExplicitRoutes(registry, subroute.Parents);
                RegisterExplicitRoutes(registry, subroute.Children);
            }
        }

        private void RegisterExplicitRoutes(ResourceRegistry registry, List<string> patterns)
        {
            if (patterns == null)
                return;

            foreach (var pattern in patterns)
            {
                if (string.IsNullOrEmpty(pattern) || pattern.Contains(Wildcard))
                    continue;

                // Register the route by adding it as a resource with itself as a route
                // This ensures it appears in GetRouteUrls()
                registry.AddResource(pattern, new List<string> { pattern });
            }
        }

        /// <summary>
        /// Expands and stores subroutes for each route in the registry.
        /// For each route, finds which subroute rules have parent patterns that match,
        /// and stores the corresponding child patterns.
        /// </summary>
        private void ExpandSubroutes(ResourceRegistry registry, LoadContext context)
        {
            if (context.RouteSettings?.Subroutes == null || context.RouteSettings.Subroutes.Count == 0)
                return;

            foreach (var routeUrl in context.AllRouteUrls)
            {
                var children = new List<string>();

                foreach (var subroute in context.RouteSettings.Subroutes)
                {
                    if (subroute.Parents == null || subroute.Children == null)
                        continue;

                    foreach (var parentPattern in subroute.Parents)
                    {
                        if (MatchesPattern(routeUrl, parentPattern))
                        {
                            children.AddRange(subroute.Children);
                            break;
                        }
                    }
                }

                if (children.Count > 0)
                {
                    registry.SetSubroutes(routeUrl, children.Distinct().OrderBy(x => x).ToList());
                }
            }
        }

        public void Load(PermissionMatrix matrix, Guid organizationId)
        {
            var context = BuildLoadContext();

            var organization = context.Organizations
                .SingleOrDefault(o => o.OrganizationIdentifier == organizationId);

            if (organization == null)
                return;

            matrix.Resources.AddResources(context.Resources); // Add resources to the shared registry (idempotent)

            // Register routes from RouteSettings in the shared registry
            RegisterRoutesFromSettings(matrix.Resources, context.RouteSettings);

            // Expand and store subroutes for each route
            ExpandSubroutes(matrix.Resources, context);

            var list = matrix.GetOrCreatePermissions(organization.OrganizationCode);

            list.Clear();

            LoadOrganization(matrix, organization, context);
        }

        private LoadContext BuildLoadContext()
        {
            var routes = GetRoutes();
            var resources = BuildResources(routes);
            var organizations = GetActiveOrganizations();
            var allPermissions = GetPermissionsGrantedOnActions();
            var routeSettings = GetRouteSettings();

            return new LoadContext(routes, resources, organizations, allPermissions, routeSettings);
        }

        private void LoadOrganization(PermissionMatrix matrix, OrganizationInfo organization, LoadContext context)
        {
            var list = matrix.GetOrCreatePermissions(organization.OrganizationCode);
            var resourcePermissions = new List<ResourcePermissions>();
            var permissionsByResource = new Dictionary<string, ResourcePermissions>(StringComparer.OrdinalIgnoreCase);

            if (context.PermissionsByOrg.TryGetValue(organization.OrganizationIdentifier, out var permissions))
            {
                foreach (var permission in permissions)
                {
                    var resourcePath = permission.Resource;

                    if (!permissionsByResource.TryGetValue(resourcePath, out var resourcePermission))
                    {
                        resourcePermission = new ResourcePermissions { Resource = resourcePath };
                        permissionsByResource[resourcePath] = resourcePermission;
                        resourcePermissions.Add(resourcePermission);
                    }

                    var bundle = new RoleAccessBundle();

                    if (permission.Role != null)
                        bundle.Roles.Add(permission.Role);

                    var data = new DataAccessHelper(bundle.Access.Data);

                    if (permission.AllowRead) data.Add(DataAccess.Read);
                    if (permission.AllowWrite) data.Add(DataAccess.Update);
                    if (permission.AllowCreate) data.Add(DataAccess.Create);
                    if (permission.AllowDelete) data.Add(DataAccess.Delete);
                    if (permission.AllowAdministrate) data.Add(DataAccess.Administrate);
                    if (permission.AllowConfigure) data.Add(DataAccess.Configure);

                    bundle.Access.Data = data.Value;
                    resourcePermission.Permissions.Add(bundle);
                }
            }

            AddExtraPermissionsGranted(organization.OrganizationIdentifier, resourcePermissions);
            AddSystemAccessRules(resourcePermissions);

            // Apply subroute inheritance: child routes inherit permissions from parent routes
            ApplySubrouteInheritance(resourcePermissions, context);

            var grantedPermissions = context.PermissionListLoader.ExpandWildcards(resourcePermissions.ToArray());
            context.PermissionListLoader.Populate(grantedPermissions, list, AccessOperation.Grant);

            var denyPermissions = new List<ResourcePermissions>();
            AddExtraPermissionsDenied(organization.OrganizationIdentifier, denyPermissions);

            var deniedPermissions = context.PermissionListLoader.ExpandWildcards(denyPermissions.ToArray());
            context.PermissionListLoader.Populate(deniedPermissions, list, AccessOperation.Deny);
        }

        #endregion

        #region Load context

        private class LoadContext
        {
            public IReadOnlyList<Resource> Resources { get; }
            public IReadOnlyList<OrganizationInfo> Organizations { get; }
            public Dictionary<string, RouteEndpoint> RouteByUrl { get; }
            public Dictionary<string, Resource> ResourceByRoute { get; }
            public Dictionary<Guid, List<GroupPermissionInfo>> PermissionsByOrg { get; }
            public PermissionListLoader PermissionListLoader { get; }
            public RouteSettings RouteSettings { get; }
            public HashSet<string> AllRouteUrls { get; }

            public LoadContext(
                IReadOnlyList<RouteEndpoint> routes,
                IReadOnlyList<Resource> resources,
                IReadOnlyList<OrganizationInfo> organizations,
                IReadOnlyList<(Guid OrganizationId, GroupPermissionInfo Permission)> allPermissions,
                RouteSettings routeSettings)
            {
                Resources = resources;
                Organizations = organizations;
                RouteSettings = routeSettings;

                RouteByUrl = routes.ToDictionary(r => r.RouteUrl, r => r, StringComparer.OrdinalIgnoreCase);
                AllRouteUrls = new HashSet<string>(routes.Select(r => r.RouteUrl), StringComparer.OrdinalIgnoreCase);

                // Add explicit routes from RouteSettings (non-wildcard patterns)
                AddRoutesFromSettings(routeSettings, AllRouteUrls);

                ResourceByRoute = new Dictionary<string, Resource>(StringComparer.OrdinalIgnoreCase);
                foreach (var resource in resources)
                {
                    foreach (var routeUrl in resource.Routes)
                    {
                        ResourceByRoute[routeUrl] = resource;
                    }
                    ResourceByRoute[resource.Path] = resource;
                }

                PermissionsByOrg = allPermissions
                    .GroupBy(x => x.OrganizationId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Permission).ToList());

                PermissionListLoader = new PermissionListLoader();
                PermissionListLoader.AddResources(resources);

                // Register routes from RouteSettings so they can be matched by wildcards
                foreach (var routeUrl in AllRouteUrls)
                {
                    PermissionListLoader.AddRoute(routeUrl);
                }
            }

            /// <summary>
            /// Extracts explicit (non-wildcard) routes from RouteSettings and adds them to the route collection.
            /// This ensures routes defined in configuration are known even if not in the database.
            /// </summary>
            private static void AddRoutesFromSettings(RouteSettings settings, HashSet<string> routeUrls)
            {
                if (settings?.Subroutes == null)
                    return;

                foreach (var subroute in settings.Subroutes)
                {
                    // Add explicit parent routes (non-wildcard)
                    if (subroute.Parents != null)
                    {
                        foreach (var parent in subroute.Parents)
                        {
                            if (!string.IsNullOrEmpty(parent) && !parent.Contains("*"))
                                routeUrls.Add(parent);
                        }
                    }

                    // Add explicit child routes (non-wildcard)
                    if (subroute.Children != null)
                    {
                        foreach (var child in subroute.Children)
                        {
                            if (!string.IsNullOrEmpty(child) && !child.Contains("*"))
                                routeUrls.Add(child);
                        }
                    }
                }
            }
        }

        private void AddExtraPermissionsGranted(Guid organizationId, List<ResourcePermissions> resourcePermissions)
        {
            var permissions = GetExtraPermissionsGranted(organizationId);

            foreach (var permission in permissions)
            {
                var resourcePermission = resourcePermissions.SingleOrDefault(x => x.Resource == permission.Resource);

                if (resourcePermission == null)
                {
                    resourcePermissions.Add(permission);
                    continue;
                }

                foreach (var r in permission.Routes)
                    resourcePermission.Routes.Add(r);

                foreach (var p in permission.Permissions)
                    resourcePermission.Permissions.Add(p);
            }
        }

        /// <summary>
        /// Applies subroute inheritance rules. Child routes inherit permissions from parent routes.
        /// </summary>
        private void ApplySubrouteInheritance(List<ResourcePermissions> resourcePermissions, LoadContext context)
        {
            if (context.RouteSettings?.Subroutes == null || context.RouteSettings.Subroutes.Count == 0)
                return;

            // Build a lookup of route URL to its permissions for efficient matching
            var permissionsByRoute = new Dictionary<string, List<RoleAccessBundle>>(StringComparer.OrdinalIgnoreCase);
            foreach (var rp in resourcePermissions)
            {
                // Add permissions for the resource path itself
                if (!permissionsByRoute.ContainsKey(rp.Resource))
                    permissionsByRoute[rp.Resource] = new List<RoleAccessBundle>();
                permissionsByRoute[rp.Resource].AddRange(rp.Permissions);

                // Add permissions for each route
                foreach (var route in rp.Routes)
                {
                    if (!permissionsByRoute.ContainsKey(route))
                        permissionsByRoute[route] = new List<RoleAccessBundle>();
                    permissionsByRoute[route].AddRange(rp.Permissions);
                }
            }

            // Process each subroute rule
            foreach (var subroute in context.RouteSettings.Subroutes)
            {
                if (subroute.Parents == null || subroute.Children == null)
                    continue;

                // Find all permissions that match the parent patterns
                var parentPermissions = new List<RoleAccessBundle>();

                foreach (var parentPattern in subroute.Parents)
                {
                    foreach (var kvp in permissionsByRoute)
                    {
                        if (MatchesPattern(kvp.Key, parentPattern))
                        {
                            parentPermissions.AddRange(kvp.Value);
                        }
                    }
                }

                if (parentPermissions.Count == 0)
                    continue;

                // Apply parent permissions to all matching child routes
                foreach (var childPattern in subroute.Children)
                {
                    // Find all known routes that match the child pattern
                    var matchingChildRoutes = context.AllRouteUrls
                        .Where(route => MatchesPattern(route, childPattern))
                        .ToList();

                    foreach (var childRoute in matchingChildRoutes)
                    {
                        // Find or create a ResourcePermissions entry for this child route
                        var childPermission = resourcePermissions
                            .FirstOrDefault(rp => rp.Routes.Contains(childRoute, StringComparer.OrdinalIgnoreCase)
                                || string.Equals(rp.Resource, childRoute, StringComparison.OrdinalIgnoreCase));

                        if (childPermission == null)
                        {
                            childPermission = new ResourcePermissions
                            {
                                Resource = childRoute,
                                Routes = new List<string> { childRoute }
                            };
                            resourcePermissions.Add(childPermission);
                        }

                        // Add inherited permissions (clone to avoid shared references)
                        foreach (var parentPerm in parentPermissions)
                        {
                            var inheritedBundle = new RoleAccessBundle
                            {
                                Roles = parentPerm.Roles.ToList(),
                                Access = new AccessSet
                                {
                                    Feature = parentPerm.Access.Feature,
                                    Data = parentPerm.Access.Data,
                                    Authority = parentPerm.Access.Authority
                                }
                            };

                            childPermission.Permissions.Add(inheritedBundle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a route matches a pattern that may contain wildcards (*).
        /// </summary>
        private bool MatchesPattern(string route, string pattern)
        {
            if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(pattern))
                return false;

            if (pattern == Wildcard)
                return true;

            if (!pattern.Contains(Wildcard))
                return string.Equals(route, pattern, StringComparison.OrdinalIgnoreCase);

            // Convert wildcard pattern to regex
            var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace("\\*", ".*") + "$";

            return System.Text.RegularExpressions.Regex.IsMatch(route, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private void AddExtraPermissionsDenied(Guid organizationId, List<ResourcePermissions> resourcePermissions)
        {
            var permissions = GetExtraPermissionsDenied(organizationId);

            foreach (var permission in permissions)
            {
                var resourcePermission = resourcePermissions.SingleOrDefault(x => x.Resource == permission.Resource);

                if (resourcePermission == null)
                {
                    resourcePermissions.Add(permission);
                    continue;
                }

                foreach (var r in permission.Routes)
                    resourcePermission.Routes.Add(r);

                foreach (var p in permission.Permissions)
                    resourcePermission.Permissions.Add(p);
            }
        }

        #endregion

        #region System access rules

        /// <summary>
        /// Applies partition-wide access control rules. Override to customize.
        /// </summary>
        protected virtual void AddSystemAccessRules(List<ResourcePermissions> resourcePermissions)
        {
            // Explicitly grant full access on all resources to all system operators.
            GrantAccess(Wildcard, SystemRole.Operator, Wildcard, resourcePermissions);

            // Explicitly grant portal access to all learners.
            GrantAccess("Portal", SystemRole.Learner, "data:read", resourcePermissions);

            // TODO: Instead of having access control rules above hardcoded like this, instead we should read the list
            // of partition-wide access control rules from a configuration file. This will allow permission changes
            // without code changes.
        }

        protected void GrantAccess(string resource, string role, string access, List<ResourcePermissions> resourcePermissions)
        {
            var roleAccess = new RoleAccessBundle();
            roleAccess.Roles.Add(role);
            roleAccess.Access.Add(access);

            var newPermissions = new ResourcePermissions();
            newPermissions.Resource = resource;
            newPermissions.Permissions.Add(roleAccess);

            resourcePermissions.Add(newPermissions);
        }

        #endregion

        #region Resource building

        protected IReadOnlyList<Resource> BuildResources(IReadOnlyList<RouteEndpoint> entries)
        {
            // Separate resources (depth 0) from routes (depth > 0)

            var resources = entries.Where(e => e.RouteDepth == 0).ToList();

            var routes = entries.Where(e => e.RouteDepth > 0).ToList();

            // Group routes by their parent resource

            var parentRoutes = routes
                .Where(e => e.ParentRouteId.HasValue)
                .GroupBy(e => e.ParentRouteId.Value)
                .ToDictionary(g => g.Key, g => g.Select(e => e.RouteUrl).ToList());

            // Build resource list

            var list = new List<Resource>();

            foreach (var resource in resources)
            {
                var resourcePath = resource.ResourcePath ?? resource.RouteUrl;

                var item = new Resource(resourcePath);

                if (resource.RouteUrl != null && resource.RouteUrl != resourcePath)
                    item.AddRoute(resource.RouteUrl);

                if (parentRoutes.TryGetValue(resource.RouteId, out var itemRoutes))
                    item.AddRoutes(itemRoutes);

                list.Add(item);
            }

            // Add resources that might not exist in the Endpoint database table. If a resource has no binding to any
            // database table, then it will be missed above.

            var assumedResources = new List<string> { "setup/routing", "timeline/queries", "variant/cmds" };

            foreach (var assumedResource in assumedResources)
            {
                if (!list.Any(x => x.Path == assumedResource))
                {
                    list.Add(new Resource(assumedResource));
                }
            }

            return list;
        }

        #endregion
    }

    #region Data transfer objects

    /// <summary>
    /// Lightweight DTO for organization data needed by the permission loader.
    /// </summary>
    public class OrganizationInfo
    {
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationCode { get; set; }
    }

    /// <summary>
    /// Flattened DTO for group permission data, avoiding EF navigation property dependencies.
    /// </summary>
    public class GroupPermissionInfo
    {
        public string Resource { get; set; }
        public string Role { get; set; }

        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowAdministrate { get; set; }
        public bool AllowConfigure { get; set; }
    }

    public class RouteSettings
    {
        public List<Subroute> Subroutes { get; set; }
    }

    /// <summary>
    /// Defines a parent/child relationship between routes.
    /// Child routes inherit permissions from their parent routes.
    /// </summary>
    public class Subroute
    {
        /// <summary>
        /// Optional comment describing the purpose of this subroute rule.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Parent route patterns. Permissions granted on these routes will be inherited by children.
        /// Supports wildcard pattern matching (e.g., "client/admin/records/gradebooks/*").
        /// </summary>
        public List<string> Parents { get; set; } = new List<string>();

        /// <summary>
        /// Child route patterns that inherit permissions from the parent routes.
        /// Supports wildcard pattern matching (e.g., "api/progress/gradebooks/*").
        /// </summary>
        public List<string> Children { get; set; } = new List<string>();
    }

    #endregion
}
