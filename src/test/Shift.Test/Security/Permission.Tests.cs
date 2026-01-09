using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Test.Security;

public class PermissionTests
{
    [Fact]
    public void DefaultConstructor_InitializesWithEmptyValues()
    {
        var permission = new Permission();

        Assert.NotNull(permission.Resource);
        Assert.NotNull(permission.Role);
        Assert.NotNull(permission.Access);
        Assert.Equal(string.Empty, permission.Resource.Name);
        Assert.Equal(string.Empty, permission.Role.Name);
    }

    [Fact]
    public void StringConstructor_SetsResourceAndRole()
    {
        var permission = new Permission("documents", "admin");

        Assert.Equal("documents", permission.Resource.Name);
        Assert.Equal("admin", permission.Role.Name);
    }

    [Fact]
    public void ObjectConstructor_SetsResourceAndRole()
    {
        var resource = new Resource("reports");
        var role = new Role("manager");

        var permission = new Permission(resource, role);

        Assert.Same(resource, permission.Resource);
        Assert.Same(role, permission.Role);
    }

    [Fact]
    public void Constructor_InitializesAccessControl()
    {
        var permission = new Permission("resource", "role");

        Assert.NotNull(permission.Access);
        Assert.False(permission.Access.IsAllowed());
    }

    [Fact]
    public void Access_CanBeModified()
    {
        var permission = new Permission("resource", "role");
        permission.Access.Grant(SwitchAccess.On);

        Assert.True(permission.Access.IsAllowed());
    }

    [Fact]
    public void Access_CanAddOperationAccess()
    {
        var permission = new Permission("resource", "role");
        permission.Access.Grant(OperationAccess.Read);

        Assert.True(permission.Access.IsAllowed(OperationAccess.Read));
        Assert.False(permission.Access.IsAllowed(OperationAccess.Write));
    }
}

public class PermissionBundleTests
{
    [Fact]
    public void DefaultConstructor_InitializesEmptyLists()
    {
        var bundle = new PermissionBundle();

        Assert.NotNull(bundle.Resources);
        Assert.NotNull(bundle.Roles);
        Assert.NotNull(bundle.Access);
        Assert.Empty(bundle.Resources);
        Assert.Empty(bundle.Roles);
        Assert.Empty(bundle.Access);
    }

    [Fact]
    public void Flatten_WithNoResources_ThrowsInvalidOperationException()
    {
        var bundle = new PermissionBundle();
        bundle.Roles.Add("admin");

        var ex = Assert.Throws<InvalidOperationException>(() => bundle.Flatten());
        Assert.Contains("resource", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Flatten_WithNoRoles_ThrowsInvalidOperationException()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.Add("documents");

        var ex = Assert.Throws<InvalidOperationException>(() => bundle.Flatten());
        Assert.Contains("role", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Flatten_SingleResourceSingleRole_ReturnsSinglePermission()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.Add("documents");
        bundle.Roles.Add("admin");

        var result = bundle.Flatten();

        Assert.Single(result);
        Assert.Equal("documents", result[0].Resource.Name);
        Assert.Equal("admin", result[0].Role.Name);
    }

    [Fact]
    public void Flatten_MultipleResourcesMultipleRoles_ReturnsCartesianProduct()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.AddRange(new[] { "documents", "reports" });
        bundle.Roles.AddRange(new[] { "admin", "user", "guest" });

        var result = bundle.Flatten();

        Assert.Equal(6, result.Count); // 2 resources × 3 roles

        Assert.Contains(result, p => p.Resource.Name == "documents" && p.Role.Name == "admin");
        Assert.Contains(result, p => p.Resource.Name == "documents" && p.Role.Name == "user");
        Assert.Contains(result, p => p.Resource.Name == "documents" && p.Role.Name == "guest");
        Assert.Contains(result, p => p.Resource.Name == "reports" && p.Role.Name == "admin");
        Assert.Contains(result, p => p.Resource.Name == "reports" && p.Role.Name == "user");
        Assert.Contains(result, p => p.Resource.Name == "reports" && p.Role.Name == "guest");
    }

    [Fact]
    public void Flatten_WithNoAccessRules_DefaultsToSwitchOn()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.Add("documents");
        bundle.Roles.Add("admin");

        var result = bundle.Flatten();

        Assert.True(result[0].Access.IsAllowed());
        Assert.True(result[0].Access.IsGranted(SwitchAccess.On));
    }

    [Fact]
    public void Flatten_WithExplicitAccessRules_AppliesRules()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.Add("documents");
        bundle.Roles.Add("admin");
        bundle.Access.Add("allow:operation:read,write");

        var result = bundle.Flatten();

        Assert.True(result[0].Access.IsAllowed(OperationAccess.Read));
        Assert.True(result[0].Access.IsAllowed(OperationAccess.Write));
        Assert.False(result[0].Access.IsAllowed(OperationAccess.Delete));
    }

    [Fact]
    public void Flatten_WithMultipleAccessRules_AppliesAll()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.Add("documents");
        bundle.Roles.Add("admin");
        bundle.Access.Add("allow:operation:read");
        bundle.Access.Add("allow:http:get,post");
        bundle.Access.Add("deny:operation:delete");

        var result = bundle.Flatten();

        Assert.True(result[0].Access.IsAllowed(OperationAccess.Read));
        Assert.True(result[0].Access.IsAllowed(HttpAccess.Get));
        Assert.True(result[0].Access.IsAllowed(HttpAccess.Post));
        Assert.False(result[0].Access.IsAllowed(OperationAccess.Delete));
    }

    [Fact]
    public void Flatten_EachPermissionGetsIndependentAccessControl()
    {
        var bundle = new PermissionBundle();
        bundle.Resources.AddRange(new[] { "doc1", "doc2" });
        bundle.Roles.Add("admin");
        bundle.Access.Add("allow:operation:read");

        var result = bundle.Flatten();

        Assert.NotSame(result[0].Access, result[1].Access);

        result[0].Access.Grant(OperationAccess.Write);

        Assert.True(result[0].Access.IsAllowed(OperationAccess.Write));
        Assert.False(result[1].Access.IsAllowed(OperationAccess.Write));
    }

    [Fact]
    public void Add_Permission_AddsResourceAndRoleNames()
    {
        var bundle = new PermissionBundle();
        var permission = new Permission("documents", "admin");

        bundle.Add(permission);

        Assert.Contains("documents", bundle.Resources);
        Assert.Contains("admin", bundle.Roles);
    }

    [Fact]
    public void Add_MultiplePermissions_AccumulatesNamesWithDuplicates()
    {
        var bundle = new PermissionBundle();
        bundle.Add(new Permission("documents", "admin"));
        bundle.Add(new Permission("documents", "user"));
        bundle.Add(new Permission("reports", "admin"));

        Assert.Equal(3, bundle.Resources.Count);
        Assert.Equal(3, bundle.Roles.Count);
        Assert.Equal(2, bundle.Resources.Count(r => r == "documents"));
        Assert.Equal(2, bundle.Roles.Count(r => r == "admin"));
    }
}

public class PermissionListTests
{
    [Fact]
    public void DefaultConstructor_CreatesEmptyList()
    {
        var list = new PermissionList();

        Assert.False(list.ContainsResource("anything"));
        Assert.False(list.ContainsRole("anything"));
    }

    [Fact]
    public void Add_Permission_StoresPermission()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);

        list.Add(permission);

        var found = list.FindPermission("documents", "admin");
        Assert.NotNull(found);
        Assert.True(found.Access.IsAllowed());
    }

    [Fact]
    public void Add_DuplicatePermission_MergesAccess()
    {
        var list = new PermissionList();

        var perm1 = new Permission("documents", "admin");
        perm1.Access.Grant(OperationAccess.Read);
        list.Add(perm1);

        var perm2 = new Permission("documents", "admin");
        perm2.Access.Grant(OperationAccess.Write);
        list.Add(perm2);

        var found = list.FindPermission("documents", "admin");
        Assert.True(found.Access.IsAllowed(OperationAccess.Read));
        Assert.True(found.Access.IsAllowed(OperationAccess.Write));
    }

    [Fact]
    public void Add_PermissionBundle_FlattensAndAdds()
    {
        var list = new PermissionList();
        var bundle = new PermissionBundle();
        bundle.Resources.AddRange(new[] { "documents", "reports" });
        bundle.Roles.AddRange(new[] { "admin", "user" });
        bundle.Access.Add("allow:operation:read");

        list.Add(bundle);

        Assert.NotNull(list.FindPermission("documents", "admin"));
        Assert.NotNull(list.FindPermission("documents", "user"));
        Assert.NotNull(list.FindPermission("reports", "admin"));
        Assert.NotNull(list.FindPermission("reports", "user"));
    }

    [Fact]
    public void AddResource_String_CreatesResource()
    {
        var list = new PermissionList();

        var resource = list.AddResource("documents");

        Assert.NotNull(resource);
        Assert.Equal("documents", resource.Name);
        Assert.True(list.ContainsResource("documents"));
    }

    [Fact]
    public void AddResource_DuplicateName_ReturnsSameResource()
    {
        var list = new PermissionList();

        var resource1 = list.AddResource("documents");
        var resource2 = list.AddResource("documents");

        Assert.Same(resource1, resource2);
    }

    [Fact]
    public void AddResource_WithAliases_AddsAliases()
    {
        var list = new PermissionList();
        var aliases = new List<string> { "docs", "files" };

        list.AddResource("documents", aliases);

        var resource = list.FindResource("documents");
        Assert.Contains("docs", resource.Aliases);
        Assert.Contains("files", resource.Aliases);
    }

    [Fact]
    public void AddResources_MultipleStrings_AddsAll()
    {
        var list = new PermissionList();

        list.AddResources(new List<string> { "documents", "reports", "settings" });

        Assert.True(list.ContainsResource("documents"));
        Assert.True(list.ContainsResource("reports"));
        Assert.True(list.ContainsResource("settings"));
    }

    [Fact]
    public void AddResource_ResourceObject_AddsResource()
    {
        var list = new PermissionList();
        var resource = new Resource("documents");
        resource.AddAliases(new List<string> { "docs" });

        list.AddResource(resource);

        var found = list.FindResource("documents");
        Assert.NotNull(found);
        Assert.Contains("docs", found.Aliases);
    }

    [Fact]
    public void AddResources_ResourceObjects_AddsAll()
    {
        var list = new PermissionList();
        var resources = new List<Resource>
        {
            new Resource("documents"),
            new Resource("reports")
        };

        list.AddResources(resources);

        Assert.True(list.ContainsResource("documents"));
        Assert.True(list.ContainsResource("reports"));
    }

    [Fact]
    public void AddRole_String_CreatesRole()
    {
        var list = new PermissionList();

        var role = list.AddRole("admin");

        Assert.NotNull(role);
        Assert.Equal("admin", role.Name);
        Assert.True(list.ContainsRole("admin"));
    }

    [Fact]
    public void AddRole_DuplicateName_ReturnsSameRole()
    {
        var list = new PermissionList();

        var role1 = list.AddRole("admin");
        var role2 = list.AddRole("admin");

        Assert.Same(role1, role2);
    }

    [Fact]
    public void AddRole_RoleObject_AddsRole()
    {
        var list = new PermissionList();
        var role = new Role("admin");

        list.AddRole(role);

        Assert.True(list.ContainsRole("admin"));
    }

    [Fact]
    public void ContainsResource_WhenExists_ReturnsTrue()
    {
        var list = new PermissionList();
        list.AddResource("documents");

        Assert.True(list.ContainsResource("documents"));
    }

    [Fact]
    public void ContainsResource_WhenNotExists_ReturnsFalse()
    {
        var list = new PermissionList();

        Assert.False(list.ContainsResource("documents"));
    }

    [Fact]
    public void ContainsRole_WhenExists_ReturnsTrue()
    {
        var list = new PermissionList();
        list.AddRole("admin");

        Assert.True(list.ContainsRole("admin"));
    }

    [Fact]
    public void ContainsRole_WhenNotExists_ReturnsFalse()
    {
        var list = new PermissionList();

        Assert.False(list.ContainsRole("admin"));
    }

    [Fact]
    public void IsAllowed_StringResourceAndRole_WhenGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        Assert.True(list.IsAllowed("documents", "admin"));
    }

    [Fact]
    public void IsAllowed_StringResourceAndRole_WhenNotGranted_ReturnsFalse()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        list.Add(permission);

        Assert.False(list.IsAllowed("documents", "admin"));
    }

    [Fact]
    public void IsAllowed_StringResourceAndRole_WhenNoPermission_ReturnsFalse()
    {
        var list = new PermissionList();

        Assert.False(list.IsAllowed("documents", "admin"));
    }

    [Fact]
    public void IsAllowed_WithStringRoleCollection_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        var roles = new[] { "user", "admin", "guest" };

        Assert.True(list.IsAllowed("documents", roles));
    }

    [Fact]
    public void IsAllowed_WithStringRoleCollection_NoRoleGranted_ReturnsFalse()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        var roles = new[] { "user", "guest" };

        Assert.False(list.IsAllowed("documents", roles));
    }

    [Fact]
    public void IsAllowed_WithRoleObjectCollection_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        var roles = new[] { new Role("user"), new Role("admin") };

        Assert.True(list.IsAllowed("documents", roles));
    }

    [Fact]
    public void IsAllowed_GuidResource_WhenGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var resource = new Resource("documents");
        var permission = new Permission(resource, new Role("admin"));
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        Assert.True(list.IsAllowed(resource.Identifier, "admin"));
    }

    [Fact]
    public void IsAllowed_GuidResourceWithRoles_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var resource = new Resource("documents");
        var permission = new Permission(resource, new Role("admin"));
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        var roles = new[] { new Role("user"), new Role("admin") };

        Assert.True(list.IsAllowed(resource.Identifier, roles));
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        Assert.True(list.IsAllowed("documents", "admin", SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenDenied_ReturnsFalse()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        permission.Access.Deny(SwitchAccess.On);
        list.Add(permission);

        Assert.False(list.IsAllowed("documents", "admin", SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_SwitchAccessWithRoles_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        Assert.True(list.IsAllowed("documents", new[] { "user", "admin" }, SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_OperationAccess_WhenGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(OperationAccess.Read);
        list.Add(permission);

        Assert.True(list.IsAllowed("documents", "admin", OperationAccess.Read));
    }

    [Fact]
    public void IsAllowed_OperationAccess_WhenNotGranted_ReturnsFalse()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(OperationAccess.Read);
        list.Add(permission);

        Assert.False(list.IsAllowed("documents", "admin", OperationAccess.Write));
    }

    [Fact]
    public void IsAllowed_OperationAccessWithRoles_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(OperationAccess.Read);
        list.Add(permission);

        Assert.True(list.IsAllowed("documents", new[] { "user", "admin" }, OperationAccess.Read));
    }

    [Fact]
    public void IsAllowed_HttpAccess_WhenGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("api/documents", "admin");
        permission.Access.Add("allow:http:get,post");
        list.Add(permission);

        Assert.True(list.IsAllowed("api/documents", "admin", HttpAccess.Get));
        Assert.True(list.IsAllowed("api/documents", "admin", HttpAccess.Post));
    }

    [Fact]
    public void IsAllowed_HttpAccess_WhenNotGranted_ReturnsFalse()
    {
        var list = new PermissionList();
        var permission = new Permission("api/documents", "admin");
        permission.Access.Add("allow:http:get");
        list.Add(permission);

        Assert.False(list.IsAllowed("api/documents", "admin", HttpAccess.Delete));
    }

    [Fact]
    public void IsAllowed_HttpAccessWithRoles_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("api/documents", "admin");
        permission.Access.Add("allow:http:get");
        list.Add(permission);

        Assert.True(list.IsAllowed("api/documents", new[] { "user", "admin" }, HttpAccess.Get));
    }

    [Fact]
    public void IsAllowed_AuthorityAccess_WhenGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("system", "superuser");
        permission.Access.Add("allow:authority:administrator,developer");
        list.Add(permission);

        Assert.True(list.IsAllowed("system", "superuser", AuthorityAccess.Administrator));
        Assert.True(list.IsAllowed("system", "superuser", AuthorityAccess.Developer));
    }

    [Fact]
    public void IsAllowed_AuthorityAccess_WhenNotGranted_ReturnsFalse()
    {
        var list = new PermissionList();
        var permission = new Permission("system", "superuser");
        permission.Access.Add("allow:authority:administrator");
        list.Add(permission);

        Assert.False(list.IsAllowed("system", "superuser", AuthorityAccess.Operator));
    }

    [Fact]
    public void IsAllowed_AuthorityAccessWithRoles_AnyRoleGranted_ReturnsTrue()
    {
        var list = new PermissionList();
        var permission = new Permission("system", "superuser");
        permission.Access.Add("allow:authority:administrator");
        list.Add(permission);

        Assert.True(list.IsAllowed("system", new[] { "user", "superuser" }, AuthorityAccess.Administrator));
    }

    [Fact]
    public void Reindex_RebuildsResourcesAndRolesFromPermissions()
    {
        var list = new PermissionList();
        var permission1 = new Permission("documents", "admin");
        permission1.Access.Grant(SwitchAccess.On);
        var permission2 = new Permission("reports", "user");
        permission2.Access.Grant(SwitchAccess.On);

        list.Add(permission1);
        list.Add(permission2);

        list.Reindex();

        Assert.True(list.ContainsResource("documents"));
        Assert.True(list.ContainsResource("reports"));
        Assert.True(list.ContainsRole("admin"));
        Assert.True(list.ContainsRole("user"));
    }

    [Fact]
    public void FindPermission_ByStrings_WhenExists_ReturnsPermission()
    {
        var list = new PermissionList();
        var permission = new Permission("documents", "admin");
        list.Add(permission);

        var found = list.FindPermission("documents", "admin");

        Assert.NotNull(found);
        Assert.Equal("documents", found.Resource.Name);
        Assert.Equal("admin", found.Role.Name);
    }

    [Fact]
    public void FindPermission_ByStrings_WhenNotExists_ReturnsNull()
    {
        var list = new PermissionList();

        var found = list.FindPermission("documents", "admin");

        Assert.Null(found);
    }

    [Fact]
    public void FindPermission_ByGuid_WhenExists_ReturnsPermission()
    {
        var list = new PermissionList();
        var resource = new Resource("documents");
        var permission = new Permission(resource, new Role("admin"));
        list.Add(permission);

        var found = list.FindPermission(resource.Identifier, "admin");

        Assert.NotNull(found);
    }

    [Fact]
    public void FindResource_ByName_WhenExists_ReturnsResource()
    {
        var list = new PermissionList();
        list.AddResource("documents");

        var found = list.FindResource("documents");

        Assert.NotNull(found);
        Assert.Equal("documents", found.Name);
    }

    [Fact]
    public void FindResource_ByName_WhenNotExists_ReturnsNull()
    {
        var list = new PermissionList();

        var found = list.FindResource("documents");

        Assert.Null(found);
    }

    [Fact]
    public void FindResource_ByGuid_WhenExists_ReturnsResource()
    {
        var list = new PermissionList();
        var resource = list.AddResource("documents");

        var found = list.FindResource(resource.Identifier);

        Assert.NotNull(found);
        Assert.Same(resource, found);
    }

    [Fact]
    public void FindRole_ByName_WhenExists_ReturnsRole()
    {
        var list = new PermissionList();
        list.AddRole("admin");

        var found = list.FindRole("admin");

        Assert.NotNull(found);
        Assert.Equal("admin", found.Name);
    }

    [Fact]
    public void FindRole_ByName_WhenNotExists_ReturnsNull()
    {
        var list = new PermissionList();

        var found = list.FindRole("admin");

        Assert.Null(found);
    }

    [Fact]
    public void FindRole_ByGuid_WhenExists_ReturnsRole()
    {
        var list = new PermissionList();
        var role = list.AddRole("admin");

        var found = list.FindRole(role.Identifier);

        Assert.NotNull(found);
        Assert.Same(role, found);
    }

    [Fact]
    public void ComplexScenario_MultiplePermissionsWithVaryingAccess()
    {
        var list = new PermissionList();

        var adminDocs = new Permission("documents", "admin");
        adminDocs.Access.Grant(OperationAccess.Read);
        adminDocs.Access.Grant(OperationAccess.Write);
        adminDocs.Access.Grant(OperationAccess.Delete);
        list.Add(adminDocs);

        var userDocs = new Permission("documents", "user");
        userDocs.Access.Grant(OperationAccess.Read);
        list.Add(userDocs);

        var guestDocs = new Permission("documents", "guest");
        guestDocs.Access.Grant(SwitchAccess.On);
        list.Add(guestDocs);

        Assert.True(list.IsAllowed("documents", "admin", OperationAccess.Delete));
        Assert.False(list.IsAllowed("documents", "user", OperationAccess.Delete));
        Assert.True(list.IsAllowed("documents", "user", OperationAccess.Read));
        Assert.True(list.IsAllowed("documents", "guest"));
        Assert.False(list.IsAllowed("documents", "guest", OperationAccess.Read));
    }
}

public class PermissionMatrixTests
{
    [Fact]
    public void DefaultConstructor_InitializesEmptyCollections()
    {
        var matrix = new PermissionMatrix();

        Assert.NotNull(matrix.Permissions);
        Assert.NotNull(matrix.Resources);
        Assert.Empty(matrix.Permissions);
        Assert.Empty(matrix.Resources);
    }

    [Fact]
    public void GetPermissions_WhenExists_ReturnsList()
    {
        var matrix = new PermissionMatrix();
        var list = new PermissionList();
        matrix.Permissions["org1"] = list;

        var result = matrix.GetPermissions("org1");

        Assert.Same(list, result);
    }

    [Fact]
    public void GetPermissions_WhenNotExists_ThrowsArgumentOutOfRangeException()
    {
        var matrix = new PermissionMatrix();

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => matrix.GetPermissions("org1"));
        Assert.Contains("org1", ex.Message);
    }

    [Fact]
    public void TryGetPermissions_WhenExists_ReturnsTrueAndList()
    {
        var matrix = new PermissionMatrix();
        var list = new PermissionList();
        matrix.Permissions["org1"] = list;

        var result = matrix.TryGetPermissions("org1", out var found);

        Assert.True(result);
        Assert.Same(list, found);
    }

    [Fact]
    public void TryGetPermissions_WhenNotExists_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();

        var result = matrix.TryGetPermissions("org1", out var found);

        Assert.False(result);
        Assert.Null(found);
    }

    [Fact]
    public void GetOrCreatePermissions_WhenNotExists_CreatesList()
    {
        var matrix = new PermissionMatrix();

        var result = matrix.GetOrCreatePermissions("org1");

        Assert.NotNull(result);
        Assert.True(matrix.Permissions.ContainsKey("org1"));
    }

    [Fact]
    public void GetOrCreatePermissions_WhenExists_ReturnsExistingList()
    {
        var matrix = new PermissionMatrix();
        var existingList = matrix.GetOrCreatePermissions("org1");

        var result = matrix.GetOrCreatePermissions("org1");

        Assert.Same(existingList, result);
    }

    [Fact]
    public void AddPermissions_Deny_WithNoAccessRules_SetsDeniedFlag()
    {
        var matrix = new PermissionMatrix();
        var resourcePermissions = new ResourcePermissions
        {
            Resource = "ui/home#test",
            Permissions = new List<RoleAccessBundle>()
        };
        resourcePermissions.Permissions.Add(new RoleAccessBundle { Roles = { "TestRole" } });

        matrix.AddPermissions(AccessOperation.Deny, resourcePermissions, "org1");

        var list = matrix.GetOrCreatePermissions("org1");
        var permission = list.FindPermission("ui/home#test", "TestRole");

        Assert.NotNull(permission);
        Assert.True(permission.Access.IsDenied());
    }

    [Fact]
    public void AddPermissions_Deny_FromJsonWithNoAccessRules_SetsDeniedFlag()
    {
        var json = @"[
        {
            ""Resource"": ""ui/home#elearning"",
            ""Permissions"": [
                {
                    ""Roles"": [
                        ""Admin Only Users"",
                        ""TSSC Only Administrators"",
                        ""TSSC Only Users""
                    ]
                }
            ]
        },
        {
            ""Resource"": ""ui/home#training-plan"",
            ""Permissions"": [
                {
                    ""Roles"": [
                        ""Admin Only Users""
                    ]
                }
            ]
        },
        {
            ""Resource"": ""ui/home#education-and-training"",
            ""Permissions"": [
                {
                    ""Roles"": [
                        ""Admin Only Users""
                    ]
                }
            ]
        },
        {
            ""Resource"": ""ui/home#dashboard"",
            ""Permissions"": [
                {
                    ""Roles"": [
                        ""Admin Only Users"",
                        ""TSSC Only Administrators"",
                        ""TSSC Only Users""
                    ]
                }
            ]
        },
        {
            ""Resource"": ""ui/home#profiles"",
            ""Permissions"": [
                {
                    ""Roles"": [
                        ""Admin Only Users"",
                        ""TSSC Only Administrators"",
                        ""TSSC Only Users""
                    ]
                }
            ]
        }
    ]";

        var matrix = new PermissionMatrix();
        var resourcePermissions = JsonConvert.DeserializeObject<ResourcePermissions[]>(json);

        matrix.AddPermissions(AccessOperation.Deny, resourcePermissions, "E03");

        var permissions = matrix.GetOrCreatePermissions("E03");

        // Verify all permissions were created
        Assert.NotNull(permissions.FindPermission("ui/home#elearning", "Admin Only Users"));
        Assert.NotNull(permissions.FindPermission("ui/home#elearning", "TSSC Only Administrators"));
        Assert.NotNull(permissions.FindPermission("ui/home#elearning", "TSSC Only Users"));
        Assert.NotNull(permissions.FindPermission("ui/home#dashboard", "TSSC Only Administrators"));

        // Verify IsDenied is set correctly for each permission
        Assert.True(permissions.FindPermission("ui/home#elearning", "Admin Only Users").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#elearning", "TSSC Only Administrators").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#elearning", "TSSC Only Users").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#training-plan", "Admin Only Users").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#education-and-training", "Admin Only Users").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#dashboard", "Admin Only Users").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#dashboard", "TSSC Only Administrators").Access.IsDenied());
        Assert.True(permissions.FindPermission("ui/home#profiles", "TSSC Only Administrators").Access.IsDenied());

        // Verify IsDenied works with role collections
        Assert.True(permissions.IsDenied("ui/home#elearning", new[] { "TSSC Only Administrators" }));
        Assert.True(permissions.IsDenied("ui/home#dashboard", new[] { "TSSC Only Administrators" }));
        Assert.True(permissions.IsDenied("ui/home#profiles", new[] { "TSSC Only Administrators" }));

        // Verify non-denied roles return false
        Assert.False(permissions.IsDenied("ui/home#elearning", new[] { "Some Other Role" }));
        Assert.False(permissions.IsDenied("ui/home#training-plan", new[] { "TSSC Only Administrators" }));
    }

    [Fact]
    public void AddPermissions_WhenNotExists_AddsList()
    {
        var matrix = new PermissionMatrix();
        var list = new PermissionList();

        matrix.AddPermissions("org1", list);

        Assert.Same(list, matrix.Permissions["org1"]);
    }

    [Fact]
    public void AddPermissions_WhenExists_ThrowsInvalidOperationException()
    {
        var matrix = new PermissionMatrix();
        var list1 = new PermissionList();
        var list2 = new PermissionList();
        matrix.AddPermissions("org1", list1);

        var ex = Assert.Throws<InvalidOperationException>(() => matrix.AddPermissions("org1", list2));
        Assert.Contains("org1", ex.Message);
    }

    [Fact]
    public void AddResource_WhenNotExists_AddsResource()
    {
        var matrix = new PermissionMatrix();

        matrix.AddResource("documents");

        Assert.True(matrix.Resources.ContainsKey("documents"));
    }

    [Fact]
    public void AddResource_WhenExists_DoesNotThrow()
    {
        var matrix = new PermissionMatrix();
        matrix.AddResource("documents");

        matrix.AddResource("documents");

        Assert.Single(matrix.Resources);
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenOrganizationExists_ChecksPermission()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        Assert.True(matrix.IsAllowed("org1", "documents", "admin", SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenOrganizationNotExists_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();

        Assert.False(matrix.IsAllowed("org1", "documents", "admin", SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenPermissionNotGranted_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("documents", "admin");
        list.Add(permission);

        Assert.False(matrix.IsAllowed("org1", "documents", "admin", SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_OperationAccess_WhenGranted_ReturnsTrue()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(OperationAccess.Read);
        list.Add(permission);

        Assert.True(matrix.IsAllowed("org1", "documents", "admin", OperationAccess.Read));
    }

    [Fact]
    public void IsAllowed_OperationAccess_WhenNotGranted_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(OperationAccess.Read);
        list.Add(permission);

        Assert.False(matrix.IsAllowed("org1", "documents", "admin", OperationAccess.Write));
    }

    [Fact]
    public void IsAllowed_HttpAccess_WhenGranted_ReturnsTrue()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("api/documents", "admin");
        permission.Access.Add("allow:http:get,post");
        list.Add(permission);

        Assert.True(matrix.IsAllowed("org1", "api/documents", "admin", HttpAccess.Get));
        Assert.True(matrix.IsAllowed("org1", "api/documents", "admin", HttpAccess.Post));
    }

    [Fact]
    public void IsAllowed_HttpAccess_WhenNotGranted_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("api/documents", "admin");
        permission.Access.Add("allow:http:get");
        list.Add(permission);

        Assert.False(matrix.IsAllowed("org1", "api/documents", "admin", HttpAccess.Delete));
    }

    [Fact]
    public void IsAllowed_AuthorityAccess_WhenGranted_ReturnsTrue()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("system", "superuser");
        permission.Access.Add("allow:authority:administrator");
        list.Add(permission);

        Assert.True(matrix.IsAllowed("org1", "system", "superuser", AuthorityAccess.Administrator));
    }

    [Fact]
    public void IsAllowed_AuthorityAccess_WhenNotGranted_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("system", "superuser");
        permission.Access.Add("allow:authority:administrator");
        list.Add(permission);

        Assert.False(matrix.IsAllowed("org1", "system", "superuser", AuthorityAccess.Operator));
    }

    [Fact]
    public void IsAllowed_WithRoleList_AnyRoleGranted_ReturnsTrue()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        var roles = new List<Role> { new Role("user"), new Role("admin") };

        Assert.True(matrix.IsAllowed("org1", "documents", roles));
    }

    [Fact]
    public void IsAllowed_WithRoleList_NoRoleGranted_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();
        var list = matrix.GetOrCreatePermissions("org1");
        var permission = new Permission("documents", "admin");
        permission.Access.Grant(SwitchAccess.On);
        list.Add(permission);

        var roles = new List<Role> { new Role("user"), new Role("guest") };

        Assert.False(matrix.IsAllowed("org1", "documents", roles));
    }

    [Fact]
    public void IsAllowed_WithRoleList_WhenOrganizationNotExists_ReturnsFalse()
    {
        var matrix = new PermissionMatrix();
        var roles = new List<Role> { new Role("admin") };

        Assert.False(matrix.IsAllowed("org1", "documents", roles));
    }

    [Fact]
    public void MultiTenant_IsolatesPermissionsBetweenOrganizations()
    {
        var matrix = new PermissionMatrix();

        var org1List = matrix.GetOrCreatePermissions("org1");
        var org1Permission = new Permission("documents", "admin");
        org1Permission.Access.Grant(OperationAccess.Read);
        org1Permission.Access.Grant(OperationAccess.Write);
        org1List.Add(org1Permission);

        var org2List = matrix.GetOrCreatePermissions("org2");
        var org2Permission = new Permission("documents", "admin");
        org2Permission.Access.Grant(OperationAccess.Read);
        org2List.Add(org2Permission);

        Assert.True(matrix.IsAllowed("org1", "documents", "admin", OperationAccess.Write));
        Assert.False(matrix.IsAllowed("org2", "documents", "admin", OperationAccess.Write));
        Assert.True(matrix.IsAllowed("org2", "documents", "admin", OperationAccess.Read));
    }

    [Fact]
    public void ComplexScenario_MultipleOrganizationsWithVaryingPermissions()
    {
        var matrix = new PermissionMatrix();

        matrix.AddResource("documents");
        matrix.AddResource("reports");
        matrix.AddResource("settings");

        var org1 = matrix.GetOrCreatePermissions("org1");
        var bundle1 = new PermissionBundle();
        bundle1.Resources.AddRange(new[] { "documents", "reports" });
        bundle1.Roles.AddRange(new[] { "admin", "user" });
        bundle1.Access.Add("allow:operation:read,write");
        org1.Add(bundle1);

        var org2 = matrix.GetOrCreatePermissions("org2");
        var bundle2 = new PermissionBundle();
        bundle2.Resources.Add("documents");
        bundle2.Roles.Add("admin");
        bundle2.Access.Add("allow:operation:read");
        bundle2.Access.Add("deny:operation:write");
        org2.Add(bundle2);

        Assert.True(matrix.IsAllowed("org1", "documents", "admin", OperationAccess.Write));
        Assert.True(matrix.IsAllowed("org1", "reports", "user", OperationAccess.Read));
        Assert.False(matrix.IsAllowed("org1", "settings", "admin", OperationAccess.Read));

        Assert.True(matrix.IsAllowed("org2", "documents", "admin", OperationAccess.Read));
        Assert.False(matrix.IsAllowed("org2", "documents", "admin", OperationAccess.Write));
        Assert.False(matrix.IsAllowed("org2", "reports", "admin", OperationAccess.Read));
    }

    [Fact]
    public void ComplexScenario_RoleWithFullAccessToAllResources()
    {
        const string TestResource = "Test Resource";

        const string TestRole = "Test Role";

        var bundle = new PermissionBundle();

        bundle.Resources.Add("*");

        bundle.Roles.Add(TestRole);

        bundle.Access.Add("allow:*");

        var matrix = new PermissionMatrix();

        var list = matrix.GetOrCreatePermissions("test");

        list.Add(bundle);

        list.AddResource(TestResource);

        Assert.True(list.IsAllowed(TestResource, TestRole));

        list.Add("*", "Platform Operator", "allow:*");

        Assert.True(list.IsAllowed(TestResource, "Platform Operator"));
    }
}