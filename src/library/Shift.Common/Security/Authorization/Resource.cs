using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class Resource
    {
        public string Path { get; set; }

        public List<string> Routes { get; set; }

        public Resource(string path)
        {
            Routes = new List<string>();

            if (path != null)
            {
                Path = path.ToLower();
            }
        }

        public void AddRoute(string route)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));

            var item = route.ToLower();

            if (Routes.Contains(item))
                return;

            Routes.Add(item);
        }

        public void AddRoutes(List<string> routes)
        {
            foreach (string route in routes)
            {
                AddRoute(route);
            }
        }

        public Guid GetIdentifier()
        {
            if (!string.IsNullOrWhiteSpace(Path))
                return UuidFactory.CreateV5(Path);

            return Guid.Empty;
        }
    }

    public class ResourceAccessBundle
    {
        public List<string> Resources { get; set; } = new List<string>();
        public AccessSet Access { get; set; } = new AccessSet();
    }

    public class RouteEndpoint
    {
        public Guid RouteId { get; set; }
        public string RouteType { get; set; }
        public string RouteUrl { get; set; }
        public int RouteDepth { get; set; }
        public Guid? ParentRouteId { get; set; }
        public string ResourcePath { get; set; }
    }

    public class RouteNavigationNode
    {
        public int RouteDepth { get; set; }
        public string RouteIcon { get; set; }
        public Guid RouteId { get; set; }
        public string RouteList { get; set; }
        public string RouteName { get; set; }
        public string RouteNameShort { get; set; }
        public string RouteType { get; set; }
        public string RouteUrl { get; set; }
        public string AuthorityType { get; set; }
        public string AuthorizationRequirement { get; set; }
        public string ControllerPath { get; set; }
        public string ExtraBreadcrumb { get; set; }
        public string HelpUrl { get; set; }
        public Guid? ParentRouteId { get; set; }
        public string SubsystemName { get; set; }
        public string ResourcePath { get; set; }
        public string SortPath { get; set; }
    }

    public class RoutePermissionNode
    {
        public int RouteDepth { get; set; }
        public string RouteIcon { get; set; }
        public Guid RouteId { get; set; }
        public string RouteList { get; set; }
        public string RouteName { get; set; }
        public string RouteNameShort { get; set; }
        public string RouteType { get; set; }
        public string RouteUrl { get; set; }
        public string AuthorityType { get; set; }
        public string AuthorizationRequirement { get; set; }
        public string ControllerPath { get; set; }
        public string ExtraBreadcrumb { get; set; }
        public string HelpUrl { get; set; }
        public Guid? ParentRouteId { get; set; }
        public string SubsystemName { get; set; }
        public string ResourcePath { get; set; }
    }

    public struct ResourceDescriptor
    {
        public string Component { get; set; }
        public string Part { get; set; }
        public string Verb { get; set; }
    }

    public class ResourcePermissions
    {
        public string Resource { get; set; }
        public List<string> Routes { get; set; } = new List<string>();
        public List<RoleAccessBundle> Permissions { get; set; } = new List<RoleAccessBundle>();

        /// <summary>
        /// Indicates this entry was created through wildcard expansion (implied by other permissions).
        /// </summary>
        public bool IsExpanded { get; set; }
    }

    public class ResourceReflector
    {
        public List<Resource> BuildResourceList(List<RoutePermissionNode> nodes, string routePrefix)
        {
            var resources = new List<Resource>();

            var reflector = new Shift.Common.Reflector();

            foreach (var node in nodes)
            {
                Resource resource = null;

                var path = node.ResourcePath;

                if (path != null)
                {
                    resource = resources.Find(x => x.Path == node.ResourcePath);
                }
                else
                {
                    var descriptor = Describe(node.RouteUrl);

                    path = descriptor.Part;

                    if (descriptor.Component != null)
                        path = descriptor.Component + "/" + path;

                    resource = resources.Find(x => x.Path == path);
                }

                if (resource == null)
                {
                    resource = new Resource(path);

                    resources.Add(resource);
                }

                path = node.RouteUrl
                    .Replace(routePrefix, string.Empty)
                    .TrimStart(new[] { '/' });

                resource.Routes.Add($"{routePrefix}/{path}");
            }

            CreateImplicitResources(resources);

            foreach (var resource in resources)
            {
                resource.Routes.Sort();
            }

            return resources
                .OrderBy(x => x.Path)
                .ToList();
        }

        public List<Resource> BuildResourceList(Type type, string routePrefix)
        {
            var resources = new List<Resource>();

            var reflector = new Shift.Common.Reflector();

            var hardcodedConstants = reflector.FindConstants(type, '.');

            foreach (var hardcodedConstant in hardcodedConstants)
            {
                var descriptor = Describe(hardcodedConstant.Value);

                var resource = resources.Find(x => x.Path == descriptor.Part);

                if (resource == null)
                {
                    resource = new Resource(descriptor.Part);

                    resources.Add(resource);
                }

                var name = hardcodedConstant.Value
                    .Replace(routePrefix, string.Empty)
                    .TrimStart(new[] { '/' });

                resource.Routes.Add($"{routePrefix}/{name}");
            }

            CreateImplicitResources(resources);

            foreach (var resource in resources)
            {
                resource.Routes.Sort();
            }

            return resources
                .OrderBy(x => x.Path)
                .ToList();
        }

        private ResourceDescriptor Describe(string name)
        {
            var descriptor = new ResourceDescriptor();

            descriptor.Part = name;

            var segments = name.Split('/');

            if (segments.Length > 0)
            {
                var first = segments.First();

                var component = ComponentHelper.Resolve(first);

                if (component != null)
                {
                    descriptor.Component = component;
                }

                var last = segments.LastOrDefault();

                if (DataAccessHelper.IsRecognized(last))
                {
                    descriptor.Part = string.Join("/", segments.Take(segments.Count() - 1));
                }
            }

            return descriptor;
        }

        private void CreateImplicitResources(List<Resource> resources)
        {
            var implicitNames = new HashSet<string>();

            foreach (var resource in resources)
            {
                var segments = resource.Path.Split('/');

                for (int i = 1; i < segments.Length; i++)
                {
                    var implicitName = string.Join("/", segments.Take(i));

                    implicitNames.Add(implicitName);
                }
            }

            foreach (var implicitName in implicitNames)
            {
                if (!resources.Any(x => x.Path == implicitName))
                {
                    resources.Add(new Resource(implicitName));
                }
            }
        }

        public bool PathImpliesRead(string path)
        {
            var standardQueryVerbs = new string[] { "Assert", "Collect", "Count", "Download", "Retrieve", "Search", "Export" };

            var legacyQueryVerbs = new string[] { "Read", "Outline", "View" };

            return StringHelper.EndsWithAny(path, standardQueryVerbs) ||
                StringHelper.EndsWithAny(path, legacyQueryVerbs);
        }

        public bool PathImpliesWrite(string path)
        {
            var standardCommandVerbs = new string[] { "Create", "Import", "Modify" };

            var legacyCommandVerbs = new string[] { "Edit", "Insert", "Update", "Upload" };

            return StringHelper.EndsWithAny(path, standardCommandVerbs) ||
                StringHelper.EndsWithAny(path, legacyCommandVerbs);
        }

        public bool PathImpliesDelete(string path)
        {
            var standardCommandVerbs = new string[] { "Delete", "Purge" };

            var legacyCommandVerbs = new string[] { "Remove", "Void" };

            return StringHelper.EndsWithAny(path, standardCommandVerbs) ||
                StringHelper.EndsWithAny(path, legacyCommandVerbs);
        }

        private string ReplaceLastPathSegment(string input, DataAccess access)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var lastSlash = input.LastIndexOf('/');

            if (lastSlash == -1 || lastSlash == input.Length - 1)
                return input;

            var output = input.Substring(0, lastSlash) + $"/{access}";

            return output.ToLower();
        }
    }

    public class RoleAccessBundle
    {
        public List<string> Roles { get; set; } = new List<string>();
        public AccessSet Access { get; set; } = new AccessSet();
    }

    public class RolePermissions
    {
        public string Role { get; set; }
        public List<ResourceAccessBundle> Permissions { get; set; } = new List<ResourceAccessBundle>();
    }
}