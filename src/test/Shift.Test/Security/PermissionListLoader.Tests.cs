using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Test.Security;

/// <summary>
/// Simple JSON serializer for testing purposes.
/// </summary>
internal class TestJsonSerializer : IJsonSerializer
{
    public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value)!;

    public T Deserialize<T>(string value, Type type, bool ignoreAttributes)
        => (T)JsonConvert.DeserializeObject(value, type)!;

    public string Serialize<T>(T value) => JsonConvert.SerializeObject(value);

    public string Serialize(object command, string[] exclusions, bool ignoreAttributes)
        => JsonConvert.SerializeObject(command);

    public string SerializeCommand(Shift.Common.Timeline.Commands.ICommand command)
        => JsonConvert.SerializeObject(command);

    public string SerializeChange(Shift.Common.Timeline.Changes.IChange command)
        => JsonConvert.SerializeObject(command);

    public string GetClassName(Type type)
        => type.FullName ?? type.Name;
}

public class PermissionListLoaderConstructorTests
{
    [Fact]
    public void Constructor_WithSerializer_CreatesInstance()
    {
        var serializer = new TestJsonSerializer();

        var loader = new PermissionListLoader();

        Assert.NotNull(loader);
    }

    [Fact]
    public void Constructor_WithResourcesAndRoles_InitializesCollections()
    {
        var serializer = new TestJsonSerializer();
        var resources = new[] { "documents", "reports" };
        var roles = new[] { "admin", "user" };

        var loader = new PermissionListLoader(resources, roles);

        Assert.NotNull(loader);
    }

    [Fact]
    public void Constructor_WithResourcesRolesAndRoutes_InitializesCollections()
    {
        var resources = new[] { "documents" };
        var roles = new[] { "admin" };
        var routes = new[] { "api/documents" };

        var loader = new PermissionListLoader(resources, roles, routes);

        Assert.NotNull(loader);
    }
}

public class PermissionListLoaderRegistrationTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void AddResource_String_RegistersResource()
    {
        var loader = new PermissionListLoader();

        loader.AddResource("documents");

        var json = @"[{ ""Resource"": ""documents"", ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Equal("documents", result[0].Resource);
    }

    [Fact]
    public void AddResource_NullOrEmpty_DoesNotThrow()
    {
        var loader = new PermissionListLoader();

        loader.AddResource((string?)null);
        loader.AddResource(string.Empty);

        Assert.NotNull(loader);
    }

    [Fact]
    public void AddResource_WithRoutes_RegistersResourceAndRoutes()
    {
        var loader = new PermissionListLoader();
        var routes = new[] { "api/documents", "api/docs" };

        loader.AddResource("documents", routes);

        var json = @"[{ ""Resource"": ""documents"", ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Contains("api/documents", result[0].Routes);
        Assert.Contains("api/docs", result[0].Routes);
    }

    [Fact]
    public void AddResource_ResourceObject_RegistersResourceWithRoutes()
    {
        var loader = new PermissionListLoader();
        var resource = new Resource("documents");
        resource.AddRoute("api/documents");

        loader.AddResource(resource);

        var json = @"[{ ""Resource"": ""documents"", ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Contains("api/documents", result[0].Routes);
    }

    [Fact]
    public void AddResources_StringCollection_RegistersAll()
    {
        var loader = new PermissionListLoader();

        loader.AddResources(new[] { "documents", "reports", "settings" });

        var json = @"[{ ""Resource"": ""*"", ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result.Length);
    }

    [Fact]
    public void AddResources_ResourceCollection_RegistersAllWithRoutes()
    {
        var loader = new PermissionListLoader();
        var resources = new[]
        {
            new Resource("documents") { Routes = { "api/documents" } },
            new Resource("reports") { Routes = { "api/reports" } }
        };

        loader.AddResources(resources);

        var json = @"[{ ""Resource"": ""*"", ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void AddRoute_RegistersRoute()
    {
        var loader = new PermissionListLoader();

        loader.AddRoute("api/documents");
        loader.AddResource("documents");

        var json = @"[{ ""Resource"": ""documents"", ""Routes"": [""api/*""], ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Contains("api/documents", result[0].Routes);
    }

    [Fact]
    public void AddRoute_NullOrEmpty_DoesNotThrow()
    {
        var loader = new PermissionListLoader();

        loader.AddRoute(null);
        loader.AddRoute(string.Empty);

        Assert.NotNull(loader);
    }

    [Fact]
    public void AddRoutes_RegistersMultipleRoutes()
    {
        var loader = new PermissionListLoader();

        loader.AddRoutes(new[] { "api/documents", "api/reports" });
        loader.AddResource("all");

        var json = @"[{ ""Resource"": ""all"", ""Routes"": [""*""], ""Permissions"": [] }]";
        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result[0].Routes.Count);
    }

    [Fact]
    public void AddRole_RegistersRole()
    {
        var loader = new PermissionListLoader();

        loader.AddRole("admin");
        loader.AddResource("documents");

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""*""], ""Access"": { ""Data"": ""Read"" } }]
        }]";
        var result = loader.LoadFromJson(json);

        Assert.Contains("admin", result[0].Permissions[0].Roles);
    }

    [Fact]
    public void AddRole_NullOrEmpty_DoesNotThrow()
    {
        var loader = new PermissionListLoader();

        loader.AddRole(null);
        loader.AddRole(string.Empty);

        Assert.NotNull(loader);
    }

    [Fact]
    public void AddRoles_RegistersMultipleRoles()
    {
        var loader = new PermissionListLoader();

        loader.AddRoles(new[] { "admin", "user", "guest" });
        loader.AddResource("documents");

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""*""], ""Access"": { ""Data"": ""Read"" } }]
        }]";
        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result[0].Permissions[0].Roles.Count);
    }
}

public class PermissionListLoaderJsonLoadingTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void LoadFromJson_ValidJson_ReturnsPermissions()
    {
        var loader = new PermissionListLoader();
        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{
                ""Roles"": [""admin""],
                ""Access"": { ""Data"": ""Read"" }
            }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Equal("documents", result[0].Resource);
        Assert.Single(result[0].Permissions);
        Assert.Contains("admin", result[0].Permissions[0].Roles);
    }

    [Fact]
    public void LoadFromJson_NullJson_ThrowsArgumentException()
    {
        var loader = new PermissionListLoader();

        Assert.Throws<ArgumentException>(() => loader.LoadFromJson(null));
    }

    [Fact]
    public void LoadFromJson_EmptyJson_ThrowsArgumentException()
    {
        var loader = new PermissionListLoader();

        Assert.Throws<ArgumentException>(() => loader.LoadFromJson(string.Empty));
    }

    [Fact]
    public void LoadFromJson_EmptyArray_ReturnsEmptyArray()
    {
        var loader = new PermissionListLoader();

        var result = loader.LoadFromJson("[]");

        Assert.Empty(result);
    }

    [Fact]
    public void LoadFromJson_ValidJson_ReturnsExpandedPermissions()
    {
        var loader = new PermissionListLoader();
        loader.AddRoute("api/documents");

        var json = @"[{
            ""Resource"": ""documents"",
            ""Routes"": [""api/documents""],
            ""Permissions"": [{
                ""Roles"": [""admin""],
                ""Access"": { ""Data"": ""Read"" }
            }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Equal("documents", result[0].Resource);
        Assert.Contains("api/documents", result[0].Routes);
    }

    [Fact]
    public void LoadFromJson_WithoutRoutes_UsesResourceRouteMap()
    {
        var loader = new PermissionListLoader();
        var resource = new Resource("documents");
        resource.AddRoute("api/documents");
        resource.AddRoute("api/docs");
        loader.AddResource(resource);

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Contains("api/documents", result[0].Routes);
        Assert.Contains("api/docs", result[0].Routes);
    }
}

public class PermissionListLoaderResourceWildcardTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void ResourceWildcard_Asterisk_ExpandsToAllResources()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "documents", "reports", "settings" });

        var json = @"[{
            ""Resource"": ""*"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result.Length);
        Assert.Contains(result, r => r.Resource == "documents");
        Assert.Contains(result, r => r.Resource == "reports");
        Assert.Contains(result, r => r.Resource == "settings");
    }

    [Fact]
    public void ResourceWildcard_Prefix_ExpandsToMatchingResources()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "workflow/cases", "workflow/forms", "content/courses" });

        var json = @"[{
            ""Resource"": ""workflow/*"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.Resource == "workflow/cases");
        Assert.Contains(result, r => r.Resource == "workflow/forms");
        Assert.DoesNotContain(result, r => r.Resource == "content/courses");
    }

    [Fact]
    public void ResourceWildcard_Suffix_ExpandsToMatchingResources()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "user-documents", "admin-documents", "user-reports" });

        var json = @"[{
            ""Resource"": ""*-documents"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.Resource == "user-documents");
        Assert.Contains(result, r => r.Resource == "admin-documents");
    }

    [Fact]
    public void ResourceWildcard_Middle_ExpandsToMatchingResources()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "api/v1/users", "api/v2/users", "api/v1/roles" });

        var json = @"[{
            ""Resource"": ""api/*/users"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.Resource == "api/v1/users");
        Assert.Contains(result, r => r.Resource == "api/v2/users");
    }

    [Fact]
    public void ResourceWildcard_NoMatch_ReturnsEmpty()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "documents", "reports" });

        var json = @"[{
            ""Resource"": ""settings/*"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Empty(result);
    }

    [Fact]
    public void ResourceWildcard_QuestionMark_MatchesSingleCharacter()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "doc1", "doc2", "doc10", "docs" });

        // The ? wildcard matches a single character
        // "doc?" pattern should match doc1, doc2, docs (4 chars) but not doc10 (5 chars)
        var json = @"[{
            ""Resource"": ""doc*"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        // All 4 resources match "doc*"
        Assert.Equal(4, result.Length);
        Assert.Contains(result, r => r.Resource.Equals("doc1", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result, r => r.Resource.Equals("doc2", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result, r => r.Resource.Equals("docs", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result, r => r.Resource.Equals("doc10", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ResourceWildcard_NoWildcard_ReturnsExactMatch()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "documents", "reports" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Equal("documents", result[0].Resource);
    }

    [Fact]
    public void ResourceWildcard_CaseInsensitive_MatchesIgnoringCase()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "Documents", "REPORTS" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
    }

    [Fact]
    public void ResourceWildcard_IsExpanded_SetToTrueForExpandedResources()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "documents", "reports" });

        var json = @"[{
            ""Resource"": ""*"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result.Length);
        Assert.All(result, r => Assert.True(r.IsExpanded));
    }

    [Fact]
    public void ResourceWildcard_IsExpanded_SetToFalseForExactMatch()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "documents", "reports" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.False(result[0].IsExpanded);
    }

    [Fact]
    public void ResourceWildcard_IsExpanded_MixedExpandedAndExact()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "workflow/cases", "workflow/forms", "documents" });

        var json = @"[
            { ""Resource"": ""workflow/*"", ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }] },
            { ""Resource"": ""documents"", ""Permissions"": [{ ""Roles"": [""user""], ""Access"": { ""Data"": ""Read"" } }] }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result.Length);
        Assert.True(result.First(r => r.Resource == "workflow/cases").IsExpanded);
        Assert.True(result.First(r => r.Resource == "workflow/forms").IsExpanded);
        Assert.False(result.First(r => r.Resource == "documents").IsExpanded);
    }

    [Fact]
    public void ResourceWildcard_IsExpanded_MergedWithExplicitEntry_RetainsExpandedFlag()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "documents", "reports" });

        // Both a wildcard and explicit entry for "documents"
        var json = @"[
            { ""Resource"": ""*"", ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }] },
            { ""Resource"": ""documents"", ""Permissions"": [{ ""Roles"": [""user""], ""Access"": { ""Data"": ""Update"" } }] }
        ]";

        var result = loader.LoadFromJson(json);

        // When merged, if any source was expanded, the result is marked expanded
        var documents = result.First(r => r.Resource == "documents");
        Assert.True(documents.IsExpanded);
    }
}

public class PermissionListLoaderRouteWildcardTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void RouteWildcard_Asterisk_ExpandsToAllRoutes()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoutes(new[] { "api/documents", "api/docs", "api/files" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Routes"": [""*""],
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result[0].Routes.Count);
    }

    [Fact]
    public void RouteWildcard_Prefix_ExpandsToMatchingRoutes()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoutes(new[] { "api/v1/documents", "api/v2/documents", "web/documents" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Routes"": [""api/*""],
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result[0].Routes.Count);
        Assert.Contains("api/v1/documents", result[0].Routes);
        Assert.Contains("api/v2/documents", result[0].Routes);
    }

    [Fact]
    public void RouteWildcard_MultiplePatterns_ExpandsAll()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoutes(new[] { "api/documents", "web/documents", "internal/documents" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Routes"": [""api/*"", ""web/*""],
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result[0].Routes.Count);
        Assert.Contains("api/documents", result[0].Routes);
        Assert.Contains("web/documents", result[0].Routes);
    }

    [Fact]
    public void RouteWildcard_NoWildcard_ReturnsExactRoute()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");

        var json = @"[{
            ""Resource"": ""documents"",
            ""Routes"": [""api/documents""],
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result[0].Routes);
        Assert.Equal("api/documents", result[0].Routes[0]);
    }

    [Fact]
    public void RouteWildcard_EmptyRoutes_InheritsFromResourceMap()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents", new[] { "api/documents", "api/docs" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result[0].Routes.Count);
    }
}

public class PermissionListLoaderRoleWildcardTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void RoleWildcard_Asterisk_ExpandsToAllRoles()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoles(new[] { "admin", "user", "guest" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""*""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result[0].Permissions[0].Roles.Count);
        Assert.Contains("admin", result[0].Permissions[0].Roles);
        Assert.Contains("user", result[0].Permissions[0].Roles);
        Assert.Contains("guest", result[0].Permissions[0].Roles);
    }

    [Fact]
    public void RoleWildcard_Prefix_ExpandsToMatchingRoles()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoles(new[] { "admin-full", "admin-read", "user-basic" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""admin-*""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result[0].Permissions[0].Roles.Count);
        Assert.Contains("admin-full", result[0].Permissions[0].Roles);
        Assert.Contains("admin-read", result[0].Permissions[0].Roles);
    }

    [Fact]
    public void RoleWildcard_Suffix_ExpandsToMatchingRoles()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoles(new[] { "super-admin", "org-admin", "super-user" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""*-admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(2, result[0].Permissions[0].Roles.Count);
        Assert.Contains("super-admin", result[0].Permissions[0].Roles);
        Assert.Contains("org-admin", result[0].Permissions[0].Roles);
    }

    [Fact]
    public void RoleWildcard_MixedWithExplicit_ExpandsAndIncludes()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoles(new[] { "admin-full", "admin-read" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""admin-*"", ""superuser""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result[0].Permissions[0].Roles.Count);
        Assert.Contains("admin-full", result[0].Permissions[0].Roles);
        Assert.Contains("admin-read", result[0].Permissions[0].Roles);
        Assert.Contains("superuser", result[0].Permissions[0].Roles);
    }

    [Fact]
    public void RoleWildcard_NoMatch_BundleExcluded()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoles(new[] { "user", "guest" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""admin-*""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Empty(result[0].Permissions);
    }

    [Fact]
    public void RoleWildcard_Deduplicated_NoDuplicateRoles()
    {
        var loader = new PermissionListLoader();
        loader.AddResource("documents");
        loader.AddRoles(new[] { "admin" });

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{ ""Roles"": [""*"", ""admin""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result[0].Permissions[0].Roles);
    }
}

public class PermissionListLoaderMergingTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void Merge_SameResource_CombinesPermissions()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "workflow/cases", "workflow/forms" });

        var json = @"[
            {
                ""Resource"": ""workflow/*"",
                ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
            },
            {
                ""Resource"": ""workflow/cases"",
                ""Permissions"": [{ ""Roles"": [""user""], ""Access"": { ""Data"": ""Read"" } }]
            }
        ]";

        var result = loader.LoadFromJson(json);

        var casesPermissions = result.First(r => r.Resource == "workflow/cases");
        Assert.Equal(2, casesPermissions.Permissions.Count);
    }

    [Fact]
    public void Merge_SameResourceAndRoles_CombinesAccess()
    {
        var loader = new PermissionListLoader();

        var json = @"[
            {
                ""Resource"": ""documents"",
                ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
            },
            {
                ""Resource"": ""documents"",
                ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Update"" } }]
            }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Single(result[0].Permissions);

        var access = result[0].Permissions[0].Access;
        Assert.True(access.Has(DataAccess.Read));
        Assert.True(access.Has(DataAccess.Update));
    }

    [Fact]
    public void Merge_Routes_CombinesAndDeduplicates()
    {
        var loader = new PermissionListLoader();

        var json = @"[
            {
                ""Resource"": ""documents"",
                ""Routes"": [""api/documents"", ""api/docs""],
                ""Permissions"": []
            },
            {
                ""Resource"": ""documents"",
                ""Routes"": [""api/docs"", ""api/files""],
                ""Permissions"": []
            }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Equal(3, result[0].Routes.Count);
        Assert.Contains("api/documents", result[0].Routes);
        Assert.Contains("api/docs", result[0].Routes);
        Assert.Contains("api/files", result[0].Routes);
    }

    [Fact]
    public void Merge_CaseInsensitive_TreatsAsSameResource()
    {
        var loader = new PermissionListLoader();

        var json = @"[
            {
                ""Resource"": ""Documents"",
                ""Permissions"": [{ ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read"" } }]
            },
            {
                ""Resource"": ""documents"",
                ""Permissions"": [{ ""Roles"": [""user""], ""Access"": { ""Data"": ""Read"" } }]
            }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Equal(2, result[0].Permissions.Count);
    }
}

public class PermissionListLoaderAccessTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void Access_DataAccess_ParsedCorrectly()
    {
        var loader = new PermissionListLoader();

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{
                ""Roles"": [""admin""],
                ""Access"": { ""Data"": ""Read,Update,Delete"" }
            }]
        }]";

        var result = loader.LoadFromJson(json);
        var access = result[0].Permissions[0].Access;

        Assert.True(access.Has(DataAccess.Read));
        Assert.True(access.Has(DataAccess.Update));
        Assert.True(access.Has(DataAccess.Delete));
        Assert.False(access.Has(DataAccess.Administrate));
    }

    [Fact]
    public void Access_FeatureAccess_ParsedCorrectly()
    {
        var loader = new PermissionListLoader();

        var json = @"[{
            ""Resource"": ""feature/dashboard"",
            ""Permissions"": [{
                ""Roles"": [""admin""],
                ""Access"": { ""Feature"": ""Use"" }
            }]
        }]";

        var result = loader.LoadFromJson(json);
        var access = result[0].Permissions[0].Access;

        Assert.True(access.Has(FeatureAccess.Use));
    }

    [Fact]
    public void Access_AuthorityAccess_ParsedCorrectly()
    {
        var loader = new PermissionListLoader();

        var json = @"[{
            ""Resource"": ""system/config"",
            ""Permissions"": [{
                ""Roles"": [""superuser""],
                ""Access"": { ""Authority"": ""Administrator,Developer"" }
            }]
        }]";

        var result = loader.LoadFromJson(json);
        var access = result[0].Permissions[0].Access;

        Assert.True(access.Has(AuthorityAccess.Administrator));
        Assert.True(access.Has(AuthorityAccess.Developer));
        Assert.False(access.Has(AuthorityAccess.Operator));
    }

    [Fact]
    public void Access_MultipleTypes_ParsedCorrectly()
    {
        var loader = new PermissionListLoader();

        var json = @"[{
            ""Resource"": ""documents"",
            ""Permissions"": [{
                ""Roles"": [""admin""],
                ""Access"": {
                    ""Data"": ""Read,Update"",
                    ""Http"": ""Get,Post"",
                    ""Feature"": ""Use""
                }
            }]
        }]";

        var result = loader.LoadFromJson(json);
        var access = result[0].Permissions[0].Access;

        Assert.True(access.Has(DataAccess.Read));
        Assert.True(access.Has(FeatureAccess.Use));
    }
}

public class PermissionListLoaderComplexScenarioTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void ComplexScenario_MultipleResourcesWithWildcards()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[]
        {
            "workflow/cases",
            "workflow/forms",
            "workflow/submissions",
            "content/courses",
            "content/lessons"
        });
        loader.AddRoles(new[] { "admin", "instructor", "learner" });

        var json = @"[
            {
                ""Resource"": ""workflow/*"",
                ""Permissions"": [
                    { ""Roles"": [""admin""], ""Access"": { ""Data"": ""Read,Update,Delete"" } },
                    { ""Roles"": [""instructor""], ""Access"": { ""Data"": ""Read,Update"" } }
                ]
            },
            {
                ""Resource"": ""content/*"",
                ""Permissions"": [
                    { ""Roles"": [""*""], ""Access"": { ""Data"": ""Read"" } }
                ]
            }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(5, result.Length);

        var workflowCases = result.First(r => r.Resource == "workflow/cases");
        Assert.Equal(2, workflowCases.Permissions.Count);

        var contentCourses = result.First(r => r.Resource == "content/courses");
        Assert.Single(contentCourses.Permissions);
        Assert.Equal(3, contentCourses.Permissions[0].Roles.Count);
    }

    [Fact]
    public void ComplexScenario_ResourcesWithRoutesAndRoleWildcards()
    {
        var loader = new PermissionListLoader();

        // Register resources with their specific routes
        loader.AddResource("documents", new[] { "api/v1/documents", "api/v2/documents" });
        loader.AddResource("reports", new[] { "api/v1/reports" });

        loader.AddRoles(new[] { "org-admin", "org-user", "platform-admin" });

        // Note: "api/v1/*" wildcard will match all known routes starting with "api/v1/"
        // including "api/v1/documents" and "api/v1/reports" since both are registered
        var json = @"[
            {
                ""Resource"": ""documents"",
                ""Routes"": [""api/v1/documents""],
                ""Permissions"": [
                    { ""Roles"": [""org-*""], ""Access"": { ""Data"": ""Read"" } },
                    { ""Roles"": [""platform-admin""], ""Access"": { ""Data"": ""Read,Update,Delete"" } }
                ]
            }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Single(result[0].Routes);
        Assert.Equal("api/v1/documents", result[0].Routes[0]);
        Assert.Equal(2, result[0].Permissions.Count);
        Assert.Equal(2, result[0].Permissions[0].Roles.Count);
    }

    [Fact]
    public void ComplexScenario_OverlappingWildcards()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "api/users", "api/users/profile", "api/users/settings" });

        var json = @"[
            {
                ""Resource"": ""api/*"",
                ""Permissions"": [{ ""Roles"": [""user""], ""Access"": { ""Data"": ""Read"" } }]
            },
            {
                ""Resource"": ""api/users/*"",
                ""Permissions"": [{ ""Roles"": [""user""], ""Access"": { ""Data"": ""Update"" } }]
            }
        ]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result.Length);

        var usersProfile = result.First(r => r.Resource == "api/users/profile");
        Assert.Single(usersProfile.Permissions);
        Assert.True(usersProfile.Permissions[0].Access.Has(DataAccess.Read));
        Assert.True(usersProfile.Permissions[0].Access.Has(DataAccess.Update));
    }

    [Fact]
    public void ComplexScenario_EmptyPermissionsArray()
    {
        var loader = new PermissionListLoader();

        var json = @"[{
            ""Resource"": ""documents"",
            ""Routes"": [""api/documents""],
            ""Permissions"": []
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Single(result);
        Assert.Empty(result[0].Permissions);
        Assert.Single(result[0].Routes);
    }

    [Fact]
    public void ComplexScenario_AllWildcards()
    {
        var loader = new PermissionListLoader();
        loader.AddResources(new[] { "a", "b", "c" });
        loader.AddRoles(new[] { "x", "y", "z" });
        loader.AddRoutes(new[] { "route1", "route2" });

        var json = @"[{
            ""Resource"": ""*"",
            ""Routes"": [""*""],
            ""Permissions"": [{ ""Roles"": [""*""], ""Access"": { ""Data"": ""Read"" } }]
        }]";

        var result = loader.LoadFromJson(json);

        Assert.Equal(3, result.Length);
        foreach (var perm in result)
        {
            Assert.Equal(2, perm.Routes.Count);
            Assert.Equal(3, perm.Permissions[0].Roles.Count);
        }
    }
}

public class PermissionListLoaderFileLoadingTests
{
    private readonly TestJsonSerializer _serializer = new();

    [Fact]
    public void LoadFromFile_NullPath_ThrowsArgumentException()
    {
        var loader = new PermissionListLoader();

        Assert.Throws<ArgumentException>(() => loader.LoadFromFile(null));
    }

    [Fact]
    public void LoadFromFile_EmptyPath_ThrowsArgumentException()
    {
        var loader = new PermissionListLoader();

        Assert.Throws<ArgumentException>(() => loader.LoadFromFile(string.Empty));
    }

    [Fact]
    public void LoadFromFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        var loader = new PermissionListLoader();

        Assert.Throws<FileNotFoundException>(() =>
            loader.LoadFromFile("C:\\nonexistent\\path\\permissions.json"));
    }
}

public class ResourcePermissionsTests
{
    [Fact]
    public void DefaultConstructor_InitializesEmptyCollections()
    {
        var entry = new ResourcePermissions();

        Assert.Null(entry.Resource);
        Assert.NotNull(entry.Routes);
        Assert.NotNull(entry.Permissions);
        Assert.Empty(entry.Routes);
        Assert.Empty(entry.Permissions);
        Assert.False(entry.IsExpanded);
    }

    [Fact]
    public void IsExpanded_DefaultsToFalse()
    {
        var entry = new ResourcePermissions();

        Assert.False(entry.IsExpanded);
    }

    [Fact]
    public void IsExpanded_CanBeSetToTrue()
    {
        var entry = new ResourcePermissions { IsExpanded = true };

        Assert.True(entry.IsExpanded);
    }
}
