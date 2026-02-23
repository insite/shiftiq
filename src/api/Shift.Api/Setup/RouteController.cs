using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Setup API: Routing")]
[Route("api/setup/routing")]
[HybridAuthorize]
public class RouteController : ShiftControllerBase
{
    private readonly IPrincipalProvider _identityService;
    private readonly AppSettings _settings;
    private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;
    private readonly PermissionCache _permissions;

    public RouteController(
        IPrincipalProvider identityService,
        AppSettings settings,
        IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider,
        PermissionCache permissions)
    {
        _identityService = identityService;
        _settings = settings;
        _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
        _permissions = permissions;
    }

    [HttpGet("permissions")]
    [HybridPermission("setup/routing", AuthorityAccess.Operator)]
    public IActionResult Permissions(string organization)
    {
        var permissions = _permissions.Matrix.GetPermissions(organization);

        var items = permissions.Items;

        return Ok(items);
    }

    [HttpGet("policies")]
    public IActionResult Policies()
    {
        var policies = GetControllerTypes()
            .SelectMany(controller =>
            {
                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                return methods.SelectMany(method =>
                    method.GetCustomAttributes<RequirePermissionAttribute>(true)
                        .Select(attr => new
                        {
                            Controller = controller.Name,
                            Action = method.Name,
                            Resource = attr.Resource,
                            Policy = attr.Policy
                        }));
            })
            .OrderBy(x => x.Resource)
            .ThenBy(x => x.Controller)
            .ThenBy(x => x.Action);

        return Ok(policies);
    }

    [HttpGet("problems")]
    public IActionResult Problems()
    {
        var expectedResources = GetControllerTypes()
            .SelectMany(GetPermissionAttributes)
            .Select(attr => attr.Resource)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var actualResources = _permissions.Matrix.Resources.GetResourceNames();

        var missingResources = expectedResources
            .Except(actualResources)
            .OrderBy(x => x)
            .ToList();

        var orphanResources = actualResources
            .Except(expectedResources)
            .OrderBy(x => x)
            .ToList();

        return Ok(new
        {
            ApiEndpointResourcesMissingFromPermissionMatrix = missingResources,
            PermissionMatrixResourcesNotUsedByAnyApiEndpoint = orphanResources
        });
    }

    [HttpGet("resources")]
    public IActionResult Resources()
    {
        var apiResources = GetControllerTypes()
            .SelectMany(GetPermissionAttributes)
            .Select(attr => attr.Resource)
            .Distinct()
            .OrderBy(x => x);

        var uiResources = _permissions.Matrix.Resources.GetResourceNames();

        var resources = apiResources.Union(uiResources).OrderBy(x => x).ToArray();

        return Ok(resources);
    }

    [HttpGet("routes")]
    public async Task<IActionResult> RoutesAsync(bool grouped = false)
    {
        if (!grouped)
        {
            var items = _apiDescriptionGroupCollectionProvider
                .ApiDescriptionGroups
                .Items
                .SelectMany(g => g.Items)
                .ToList();

            var endpoints = items.Select(a => a.RelativePath)
                .Distinct()
                .OrderBy(x => x);

            var pages = _permissions.Matrix.Resources.GetRouteUrls();

            var routes = endpoints.Union(pages).OrderBy(x => x).ToArray();

            return Ok(routes);
        }
        else
        {
            var list = new List<ResourceRouteResult>();

            var uiResources = _permissions.Matrix.Resources.ToArray();

            foreach (var uiResource in uiResources)
            {
                var item = new ResourceRouteResult();

                item.Resource = uiResource.Key;

                item.Routes = uiResource.Value.Routes
                    .Select(r => r)
                    .ToList();

                list.Add(item);
            }

            var apiSpecs = await GetApiSpecificationsAsync(null);

            foreach (var apiSpec in apiSpecs)
            {
                var policies = apiSpec.AuthorizationPolicies.ToArray();

                var route = apiSpec.Route.TrimStart('/');

                foreach (var policy in policies)
                {
                    var requirement = new AuthorizationRequirement(policy);

                    var item = list.FirstOrDefault(x => x.Resource == requirement.Resource);

                    if (item == null)
                    {
                        item = new ResourceRouteResult();

                        item.Resource = requirement.Resource;

                        list.Add(item);
                    }

                    var resourceRoute = item.Routes.FirstOrDefault(x => x == apiSpec.Route);

                    if (resourceRoute == null)
                    {
                        item.Routes.Add(route);
                    }
                }
            }

            var result = list.OrderBy(x => x.Resource);

            return Ok(result);
        }
    }

    [HttpGet("specs")]
    public async Task<IActionResult> SpecsAsync(string? path)
    {
        var specs = await GetApiSpecificationsAsync(path);

        return Ok(specs);
    }

    [HttpGet("subroutes")]
    public IActionResult SubroutesAsync(bool expanded = false)
    {
        if (!expanded)
        {
            return Ok(_settings.RouteSettings);
        }

        // Only include routes that have subroutes defined

        var result = new Dictionary<string, Dictionary<string, List<string>>>();

        var uiResources = _permissions.Matrix.Resources.ToArray();

        foreach (var uiResource in uiResources)
        {
            var routesWithSubs = new Dictionary<string, List<string>>();

            foreach (var route in uiResource.Value.Routes)
            {
                var subs = _permissions.Matrix.Resources.GetSubroutes(route);

                if (subs.Count > 0)
                {
                    routesWithSubs[route] = subs;
                }
            }

            if (routesWithSubs.Count > 0)
            {
                result[uiResource.Key] = routesWithSubs;
            }
        }

        return Ok(result);
    }

    private async Task<List<OpenApiEndpoint>> GetApiSpecificationsAsync(string? path)
    {
        var baseUrl = _settings.Shift.Api.Hosting.BaseUrl.TrimEnd('/');

        var openApiUrl = $"{baseUrl}/swagger/v2/swagger.json";

        var service = new OpenApiService();

        var specs = await service.GetEndpointsByRoutePathAsync(openApiUrl, path);

        return specs;
    }

    private static IEnumerable<Type> GetControllerTypes()
    {
        return typeof(RouteController).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ControllerBase).IsAssignableFrom(t));
    }

    private static IEnumerable<RequirePermissionAttribute> GetPermissionAttributes(Type controller)
    {
        var classAttributes = controller.GetCustomAttributes<RequirePermissionAttribute>(true);

        var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        var methodAttributes = methods.SelectMany(m => m.GetCustomAttributes<RequirePermissionAttribute>(true));

        return classAttributes.Concat(methodAttributes);
    }

    public class ResourceRouteResult
    {
        public string Resource { get; set; } = null!;
        public List<string> Routes { get; set; } = new List<string>();
    }
}
