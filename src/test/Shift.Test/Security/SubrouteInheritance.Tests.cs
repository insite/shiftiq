using Shift.Common;

namespace Shift.Test.Security;

/// <summary>
/// Unit tests for subroute inheritance logic.
/// </summary>
/// <remarks>
/// These tests verify the assumptions implied in RouteSettings.json:
/// - Wildcard pattern matching for parent and child routes
/// - Permission inheritance from parent routes to child routes
/// - Multiple parents contributing permissions to the same children
/// - Explicit routes being registered in the resource registry
/// </remarks>
public class SubrouteInheritancePatternMatchingTests
{
    [Fact]
    public void WildcardPattern_MatchesRouteWithSamePrefix()
    {
        // Verifies: "client/admin/records/gradebooks/*" matches "client/admin/records/gradebooks/search"

        var pattern = "client/admin/records/gradebooks/*";

        var route = "client/admin/records/gradebooks/search";

        var matches = MatchesPattern(route, pattern);

        Assert.True(matches);
    }

    [Fact]
    public void WildcardPattern_MatchesRouteWithDifferentSuffix()
    {
        // Verifies: "client/admin/records/gradebooks/*" matches "client/admin/records/gradebooks/outline"

        var pattern = "client/admin/records/gradebooks/*";

        var route = "client/admin/records/gradebooks/outline";

        var matches = MatchesPattern(route, pattern);

        Assert.True(matches);
    }

    [Fact]
    public void WildcardPattern_DoesNotMatchDifferentPrefix()
    {
        // Verifies: "client/admin/records/gradebooks/*" does NOT match "api/progress/gradebooks/search"

        var pattern = "client/admin/records/gradebooks/*";

        var route = "api/progress/gradebooks/search";

        var matches = MatchesPattern(route, pattern);

        Assert.False(matches);
    }

    [Fact]
    public void WildcardPattern_MatchesNestedRoutes()
    {
        // Verifies: "api/progress/gradebooks/*" matches "api/progress/gradebooks/retrieve"

        var pattern = "api/progress/gradebooks/*";

        var route = "api/progress/gradebooks/retrieve";

        var matches = MatchesPattern(route, pattern);

        Assert.True(matches);
    }

    [Fact]
    public void WildcardPattern_MatchesMultipleSegments()
    {
        // Verifies: "ui/admin/contacts/people/*" matches "ui/admin/contacts/people/search"

        var pattern = "ui/admin/contacts/people/*";

        var route = "ui/admin/contacts/people/search";

        var matches = MatchesPattern(route, pattern);

        Assert.True(matches);
    }

    [Fact]
    public void ExplicitPattern_MatchesExactRoute()
    {
        // Verifies: exact match for "client/admin/records/gradebooks/search"

        var pattern = "client/admin/records/gradebooks/search";

        var route = "client/admin/records/gradebooks/search";

        var matches = MatchesPattern(route, pattern);

        Assert.True(matches);
    }

    [Fact]
    public void ExplicitPattern_DoesNotMatchDifferentRoute()
    {
        // Verifies: "client/admin/records/gradebooks/search" does NOT match "client/admin/records/gradebooks/outline"

        var pattern = "client/admin/records/gradebooks/search";

        var route = "client/admin/records/gradebooks/outline";

        var matches = MatchesPattern(route, pattern);

        Assert.False(matches);
    }

    [Fact]
    public void PatternMatching_IsCaseInsensitive()
    {
        var pattern = "Client/Admin/Records/Gradebooks/*";

        var route = "client/admin/records/gradebooks/search";

        var matches = MatchesPattern(route, pattern);

        Assert.True(matches);
    }

    /// <summary>
    /// Helper method that replicates the pattern matching logic from PermissionMatrixLoaderBase.
    /// </summary>
    private static bool MatchesPattern(string route, string pattern)
    {
        if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(pattern))
            return false;

        if (pattern == "*")
            return true;

        if (!pattern.Contains("*"))
            return string.Equals(route, pattern, StringComparison.OrdinalIgnoreCase);

        var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
            .Replace("\\*", ".*") + "$";

        return System.Text.RegularExpressions.Regex.IsMatch(route, regexPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}

public class SubrouteInheritancePermissionFlowTests
{
    [Fact]
    public void WildcardParent_TransfersPermissionsToWildcardChild()
    {
        // Scenario from RouteSettings.json Rule 1:
        // Parent: "client/admin/records/gradebooks/*"
        // Child: "api/progress/gradebooks/*"
        // Permission on "client/admin/records/gradebooks/search" should flow to "api/progress/gradebooks/retrieve"

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string> { "client/admin/records/gradebooks/*" },
                    Children = new List<string> { "api/progress/gradebooks/*" }
                }
            }
        };

        var resources = new List<TestResource>
        {
            new TestResource
            {
                ResourcePath = "Admin/Records",
                Routes = new List<string>
                {
                    "client/admin/records/gradebooks/search",
                    "api/progress/gradebooks/retrieve"
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, resources);

        var matrix = new PermissionMatrix();

        // Permission granted on "Admin/Records" resource

        matrix.AddPermissions(AccessOperation.Grant, new ResourcePermissions
        {
            Resource = "Admin/Records",
            Routes = new List<string> { "client/admin/records/gradebooks/search" },
            Permissions = new List<RoleAccessBundle>
            {
                new RoleAccessBundle
                {
                    Roles = new List<string> { "Instructor" },
                    Access = new AccessSet { Data = DataAccess.Read }
                }
            }
        }, "TestOrg");

        loader.Load(matrix);

        var permissions = matrix.GetPermissions("TestOrg");

        // Verify the child route inherited the permission

        Assert.True(permissions.IsAllowedByRoute("api/progress/gradebooks/retrieve", "Instructor", DataAccess.Read));
    }

    [Fact]
    public void MultipleParents_BothContributePermissionsToChildren()
    {
        // Scenario from RouteSettings.json Rule 2:
        // Parents: ["client/admin/records/gradebooks/search", "client/admin/records/gradebooks/outline"]
        // Children: ["api/progress/gradebooks/retrieve", ...]
        // Permissions on EITHER parent should flow to children

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string>
                    {
                        "client/admin/records/gradebooks/search",
                        "client/admin/records/gradebooks/outline"
                    },
                    Children = new List<string>
                    {
                        "api/progress/gradebooks/retrieve"
                    }
                }
            }
        };

        var resources = new List<TestResource>
        {
            new TestResource
            {
                ResourcePath = "Admin/Records",
                Routes = new List<string>
                {
                    "client/admin/records/gradebooks/search",
                    "client/admin/records/gradebooks/outline",
                    "api/progress/gradebooks/retrieve"
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, resources);

        var matrix = new PermissionMatrix();

        // Grant different permissions on different routes within the same resource

        matrix.AddPermissions(AccessOperation.Grant, new ResourcePermissions
        {
            Resource = "Admin/Records",
            Routes = new List<string> { "client/admin/records/gradebooks/search" },
            Permissions = new List<RoleAccessBundle>
            {
                new RoleAccessBundle
                {
                    Roles = new List<string> { "SearchRole" },
                    Access = new AccessSet { Data = DataAccess.Read }
                }
            }
        }, "TestOrg");

        matrix.AddPermissions(AccessOperation.Grant, new ResourcePermissions
        {
            Resource = "Admin/Records",
            Routes = new List<string> { "client/admin/records/gradebooks/outline" },
            Permissions = new List<RoleAccessBundle>
            {
                new RoleAccessBundle
                {
                    Roles = new List<string> { "OutlineRole" },
                    Access = new AccessSet { Data = DataAccess.Read }
                }
            }
        }, "TestOrg");

        loader.Load(matrix);

        var permissions = matrix.GetPermissions("TestOrg");

        // Child should have permissions from BOTH parents

        Assert.True(permissions.IsAllowedByRoute("api/progress/gradebooks/retrieve", "SearchRole", DataAccess.Read));
        Assert.True(permissions.IsAllowedByRoute("api/progress/gradebooks/retrieve", "OutlineRole", DataAccess.Read));
    }

    [Fact]
    public void MultipleChildren_AllInheritFromParent()
    {
        // Scenario from RouteSettings.json Rule 1:
        // Parent: "client/admin/records/gradebooks/*"
        // Children: ["api/progress/gradebooks/*", "api/progress/achievements/*", "api/booking/events/*"]

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string> { "client/admin/records/gradebooks/*" },
                    Children = new List<string>
                    {
                        "api/progress/gradebooks/*",
                        "api/progress/achievements/*",
                        "api/booking/events/*"
                    }
                }
            }
        };

        var resources = new List<TestResource>
        {
            new TestResource
            {
                ResourcePath = "Admin/Records",
                Routes = new List<string>
                {
                    "client/admin/records/gradebooks/search",
                    "api/progress/gradebooks/retrieve",
                    "api/progress/achievements/retrieve",
                    "api/booking/events/retrieve"
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, resources);

        var matrix = new PermissionMatrix();

        // Permission granted on Admin/Records resource

        matrix.AddPermissions(AccessOperation.Grant, new ResourcePermissions
        {
            Resource = "Admin/Records",
            Routes = new List<string> { "client/admin/records/gradebooks/search" },
            Permissions = new List<RoleAccessBundle>
            {
                new RoleAccessBundle
                {
                    Roles = new List<string> { "Admin" },
                    Access = new AccessSet { Data = DataAccess.Read }
                }
            }
        }, "TestOrg");

        loader.Load(matrix);

        var permissions = matrix.GetPermissions("TestOrg");

        // All three child routes should inherit the permission

        Assert.True(permissions.IsAllowedByRoute("api/progress/gradebooks/retrieve", "Admin", DataAccess.Read));
        Assert.True(permissions.IsAllowedByRoute("api/progress/achievements/retrieve", "Admin", DataAccess.Read));
        Assert.True(permissions.IsAllowedByRoute("api/booking/events/retrieve", "Admin", DataAccess.Read));
    }

    [Fact]
    public void ContactPeopleRoutes_InheritPermissions()
    {
        // Scenario from RouteSettings.json Rule 3:
        // Parent: "ui/admin/contacts/people/*"
        // Child: "api/directory/people/*"

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string> { "ui/admin/contacts/people/*" },
                    Children = new List<string> { "api/directory/people/*" }
                }
            }
        };

        var resources = new List<TestResource>
        {
            new TestResource
            {
                ResourcePath = "Admin/Contacts",
                Routes = new List<string>
                {
                    "ui/admin/contacts/people/search",
                    "api/directory/people/retrieve"
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, resources);

        var matrix = new PermissionMatrix();

        // Permission granted on Admin/Contacts resource

        matrix.AddPermissions(AccessOperation.Grant, new ResourcePermissions
        {
            Resource = "Admin/Contacts",
            Routes = new List<string> { "ui/admin/contacts/people/search" },
            Permissions = new List<RoleAccessBundle>
            {
                new RoleAccessBundle
                {
                    Roles = new List<string> { "ContactManager" },
                    Access = new AccessSet { Data = DataAccess.Read | DataAccess.Update }
                }
            }
        }, "TestOrg");

        loader.Load(matrix);

        var permissions = matrix.GetPermissions("TestOrg");

        // Verify the child route inherited the permissions

        Assert.True(permissions.IsAllowedByRoute("api/directory/people/retrieve", "ContactManager", DataAccess.Read));
        Assert.True(permissions.IsAllowedByRoute("api/directory/people/retrieve", "ContactManager", DataAccess.Update));
    }

    [Fact]
    public void NoMatchingParentPermissions_ChildHasNoInheritedPermissions()
    {
        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string> { "client/admin/records/gradebooks/*" },
                    Children = new List<string> { "api/progress/gradebooks/*" }
                }
            }
        };

        var resources = new List<TestResource>
        {
            new TestResource
            {
                ResourcePath = "Admin/Records",
                Routes = new List<string>
                {
                    "client/admin/records/gradebooks/search",
                    "api/progress/gradebooks/retrieve"
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, resources);

        var matrix = new PermissionMatrix();

        // Don't add any permissions on parent

        loader.Load(matrix);

        var permissions = matrix.GetPermissions("TestOrg");

        // Child should not have any permissions since parent had none

        Assert.False(permissions.IsAllowed("api/progress/gradebooks/retrieve", "AnyRole", DataAccess.Read));
    }
}

public class SubrouteRouteRegistrationTests
{
    [Fact]
    public void ExplicitParentRoutes_RegisteredInResourceRegistry()
    {
        // From RouteSettings.json Rule 2:
        // Parents: ["client/admin/records/gradebooks/search", "client/admin/records/gradebooks/outline"]
        // Both explicit routes should appear in GetRouteUrls()

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string>
                    {
                        "client/admin/records/gradebooks/search",
                        "client/admin/records/gradebooks/outline"
                    },
                    Children = new List<string>
                    {
                        "api/progress/gradebooks/retrieve"
                    }
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, Array.Empty<string>());

        var matrix = new PermissionMatrix();

        loader.Load(matrix);

        var routeUrls = matrix.Resources.GetRouteUrls().ToList();

        Assert.Contains("client/admin/records/gradebooks/search", routeUrls);
        Assert.Contains("client/admin/records/gradebooks/outline", routeUrls);
    }

    [Fact]
    public void ExplicitChildRoutes_RegisteredInResourceRegistry()
    {
        // From RouteSettings.json Rule 2:
        // Children: ["api/progress/gradebooks/retrieve", "api/progress/achievements/retrieve", "api/booking/events/retrieve"]

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string> { "client/admin/records/gradebooks/search" },
                    Children = new List<string>
                    {
                        "api/progress/gradebooks/retrieve",
                        "api/progress/achievements/retrieve",
                        "api/booking/events/retrieve"
                    }
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, Array.Empty<string>());

        var matrix = new PermissionMatrix();

        loader.Load(matrix);

        var routeUrls = matrix.Resources.GetRouteUrls().ToList();

        Assert.Contains("api/progress/gradebooks/retrieve", routeUrls);
        Assert.Contains("api/progress/achievements/retrieve", routeUrls);
        Assert.Contains("api/booking/events/retrieve", routeUrls);
    }

    [Fact]
    public void WildcardPatterns_NotRegisteredAsRoutes()
    {
        // Wildcard patterns like "client/admin/records/gradebooks/*" should NOT appear as literal routes

        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string> { "client/admin/records/gradebooks/*" },
                    Children = new List<string> { "api/progress/gradebooks/*" }
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, Array.Empty<string>());

        var matrix = new PermissionMatrix();

        loader.Load(matrix);

        var routeUrls = matrix.Resources.GetRouteUrls().ToList();

        Assert.DoesNotContain("client/admin/records/gradebooks/*", routeUrls);
        Assert.DoesNotContain("api/progress/gradebooks/*", routeUrls);
    }

    [Fact]
    public void MixedExplicitAndWildcard_OnlyExplicitRegistered()
    {
        var routeSettings = new RouteSettings
        {
            Subroutes = new List<Subroute>
            {
                new Subroute
                {
                    Parents = new List<string>
                    {
                        "client/admin/records/gradebooks/*",     // wildcard - should NOT be registered
                        "client/admin/records/gradebooks/search" // explicit - should be registered
                    },
                    Children = new List<string>
                    {
                        "api/progress/gradebooks/*",       // wildcard - should NOT be registered
                        "api/progress/gradebooks/retrieve" // explicit - should be registered
                    }
                }
            }
        };

        var loader = new TestPermissionMatrixLoader(routeSettings, Array.Empty<string>());

        var matrix = new PermissionMatrix();

        loader.Load(matrix);

        var routeUrls = matrix.Resources.GetRouteUrls().ToList();

        Assert.Contains("client/admin/records/gradebooks/search", routeUrls);
        Assert.Contains("api/progress/gradebooks/retrieve", routeUrls);
        Assert.DoesNotContain("client/admin/records/gradebooks/*", routeUrls);
        Assert.DoesNotContain("api/progress/gradebooks/*", routeUrls);
    }
}

/// <summary>
/// Test implementation of PermissionMatrixLoaderBase that provides controlled test data.
/// </summary>
/// <remarks>
/// Models the real data structure where resources (depth 0) contain routes (depth > 0).
/// </remarks>
internal class TestPermissionMatrixLoader : PermissionMatrixLoaderBase
{
    private readonly RouteSettings _routeSettings;
    private readonly List<TestResource> _resources;

    /// <summary>
    /// Creates a loader with explicit resource definitions.
    /// </summary>
    public TestPermissionMatrixLoader(RouteSettings routeSettings, List<TestResource> resources)
    {
        _routeSettings = routeSettings;
        _resources = resources;
    }

    /// <summary>
    /// Creates a loader where each route URL becomes its own resource.
    /// </summary>
    /// <remarks>
    /// Provided for backward compatibility with simpler test cases.
    /// </remarks>
    public TestPermissionMatrixLoader(RouteSettings routeSettings, string[] knownRoutes)
        : this(routeSettings, knownRoutes.Select(route => new TestResource
        {
            ResourcePath = route,
            Routes = new List<string> { route }
        }).ToList())
    {
    }

    protected override IReadOnlyList<RouteEndpoint> GetRoutes()
    {
        var endpoints = new List<RouteEndpoint>();

        foreach (var resource in _resources)
        {
            var resourceId = Guid.NewGuid();

            // Resource entry (depth 0)
            endpoints.Add(new RouteEndpoint
            {
                RouteId = resourceId,
                RouteUrl = resource.Routes.FirstOrDefault() ?? resource.ResourcePath,
                RouteDepth = 0,
                ResourcePath = resource.ResourcePath
            });

            // Route entries (depth 1) linked to the parent resource
            foreach (var route in resource.Routes.Skip(1))
            {
                endpoints.Add(new RouteEndpoint
                {
                    RouteId = Guid.NewGuid(),
                    RouteUrl = route,
                    RouteDepth = 1,
                    ResourcePath = resource.ResourcePath,
                    ParentRouteId = resourceId
                });
            }
        }

        return endpoints;
    }

    protected override IReadOnlyList<OrganizationInfo> GetActiveOrganizations()
    {
        return new List<OrganizationInfo>
        {
            new OrganizationInfo
            {
                OrganizationIdentifier = Guid.NewGuid(),
                OrganizationCode = "TestOrg"
            }
        };
    }

    protected override IReadOnlyList<(Guid OrganizationId, GroupPermissionInfo Permission)> GetPermissionsGrantedOnActions()
    {
        return new List<(Guid, GroupPermissionInfo)>();
    }

    protected override IReadOnlyList<ResourcePermissions> GetExtraPermissionsGranted(Guid organizationId)
    {
        return new List<ResourcePermissions>();
    }

    protected override IReadOnlyList<ResourcePermissions> GetExtraPermissionsDenied(Guid organizationId)
    {
        return new List<ResourcePermissions>();
    }

    protected override RouteSettings GetRouteSettings()
    {
        return _routeSettings;
    }
}

/// <summary>
/// Test data structure representing a resource with child routes.
/// </summary>
internal class TestResource
{
    /// <summary>
    /// Uniquely identifies a resource (e.g., "Admin/Records").
    /// </summary>
    public string ResourcePath { get; set; } = null!;

    /// <summary>
    /// The list of routes belonging to this resource (e.g., "client/admin/records/gradebooks/search").
    /// </summary>
    public List<string> Routes { get; set; } = new List<string>();
}
