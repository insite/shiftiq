using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Shift.Api;

internal static class RouteScanner
{
    public static List<CustomPermissionRoute> Scan()
    {
        var items = new List<CustomPermissionRoute>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var controllerType in GetControllerTypes(Assembly.GetExecutingAssembly()))
        {
            var controllerRoutes = GetControllerRouteTemplates(controllerType);
            var methods = controllerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .OrderBy(method => method.MetadataToken);

            foreach (var method in methods)
            {
                var resources = GetHybridPermissionResources(method);
                if (resources.Count == 0)
                    continue;

                var methodRoutes = GetMethodRouteTemplates(method);
                if (methodRoutes.Count == 0)
                    continue;

                var resolvedRoutes = ResolveRoutes(controllerRoutes, methodRoutes);
                foreach (var route in resolvedRoutes)
                {
                    foreach (var resource in resources)
                    {
                        var key = $"{route}\t{resource}";
                        if (!seen.Add(key))
                            continue;

                        items.Add(new CustomPermissionRoute
                        {
                            Url = route.ToLower(),
                            Resource = resource
                        });
                    }
                }
            }
        }

        return items;
    }

    private static IEnumerable<Type> GetControllerTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
                .OrderBy(type => type.FullName);
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null && type.IsClass && !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
                .Cast<Type>()
                .OrderBy(type => type.FullName);
        }
    }

    private static List<string?> GetControllerRouteTemplates(Type controllerType)
    {
        var templates = controllerType
            .GetCustomAttributes(inherit: true)
            .OfType<IRouteTemplateProvider>()
            .Select(attribute => attribute.Template)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (templates.Count == 0)
            templates.Add(null);

        return templates;
    }

    private static List<string?> GetMethodRouteTemplates(MethodInfo method)
    {
        return method
            .GetCustomAttributes(inherit: true)
            .OfType<IRouteTemplateProvider>()
            .Select(attribute => attribute.Template)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<string> GetHybridPermissionResources(MethodInfo method)
    {
        return method
            .GetCustomAttributes(inherit: true)
            .Where(x => string.Equals(x.GetType().Name, typeof(HybridPermissionAttribute).Name))
            .Select(GetResource)
            .Where(resource => !string.IsNullOrWhiteSpace(resource))
            .Select(resource => resource!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? GetResource(object attribute)
    {
        return attribute
            .GetType()
            .GetProperty("Resource", BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(attribute) as string;
    }

    private static List<string> ResolveRoutes(IEnumerable<string?> controllerRoutes, IEnumerable<string?> methodRoutes)
    {
        var routes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var controllerRoute in controllerRoutes)
        {
            foreach (var methodRoute in methodRoutes)
            {
                var route = CombineRouteTemplates(controllerRoute, methodRoute);

                if (route != null)
                    routes.Add(route);
            }
        }

        return routes
            .OrderBy(route => route, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? CombineRouteTemplates(string? controllerRoute, string? methodRoute)
    {
        if (IsAbsoluteTemplate(methodRoute))
            return NormalizeRouteTemplate(methodRoute);

        if (string.IsNullOrWhiteSpace(methodRoute))
            return NormalizeRouteTemplate(controllerRoute);

        if (string.IsNullOrWhiteSpace(controllerRoute))
            return NormalizeRouteTemplate(methodRoute);

        return NormalizeRouteTemplate($"{NormalizeRouteTemplate(controllerRoute)}/{NormalizeRouteTemplate(methodRoute)}");
    }

    private static bool IsAbsoluteTemplate(string? template)
    {
        return !string.IsNullOrWhiteSpace(template) && (template.StartsWith('/') || template.StartsWith("~/"));
    }

    private static string? NormalizeRouteTemplate(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return null;

        var route = template.Trim();

        if (route.StartsWith("~/"))
            route = route[2..];
        else
            route = route.TrimStart('/');

        route = route.TrimEnd('/');

        if (route.Length == 0)
            return string.Empty;

        var paramIndex = route.IndexOf("/{");

        return paramIndex > 0 ? route.Substring(0, paramIndex) : route;
    }
}