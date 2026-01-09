using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class Resource
    {
        public string Name { get; set; }

        public Guid Identifier { get; set; }

        public List<string> Aliases { get; set; }

        public Resource(string name, Guid? id)
        {
            Name = name;
            Identifier = id ?? (!string.IsNullOrEmpty(name) ? UuidFactory.CreateV5(name) : Guid.Empty);
            Aliases = new List<string>();
        }

        public Resource(string name) : this(name, null) { }

        public Resource(Guid id) : this(id.ToString(), id) { }

        public void AddAliases(List<string> aliases)
        {
            foreach (string alias in aliases)
            {
                if (!Aliases.Contains(alias))
                {
                    Aliases.Add(alias);
                }
            }
        }
    }

    public class ResourceReflector
    {
        public List<Resource> BuildResourceList(Type type)
        {
            var resources = new List<Resource>();

            var reflector = new Shift.Common.Reflector();

            var hardcodedResourceNames = reflector.FindConstants(type, '.');

            foreach (var hardcodedResourceName in hardcodedResourceNames)
            {
                var policy = CreateResource(hardcodedResourceName.Value);

                var item = resources.Find(x => x.Name == policy.Name);

                if (item == null)
                {
                    resources.Add(policy);
                }
                else
                {
                    item.Aliases.Add(hardcodedResourceName.Value);
                }
            }

            var list = new List<Resource>();

            foreach (var resource in resources.OrderBy(x => x.Name))
            {
                var item = new Resource(resource.Name);

                item.Aliases = resource.Aliases.OrderBy(x => x).ToList();

                list.Add(item);
            }

            return list;
        }

        public Resource CreateResource(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var resource = new Resource(name);

            var resourceName = name;

            if (PathImpliesRead(name))
                resourceName = ReplaceLastPathSegment(name, OperationAccess.Read);

            else if (PathImpliesWrite(name))
                resourceName = ReplaceLastPathSegment(name, OperationAccess.Write);

            else if (PathImpliesDelete(name))
                resourceName = ReplaceLastPathSegment(name, OperationAccess.Delete);

            resource.Name = resourceName;

            // If the implicit policy slug differs from the explicit policy input then store the original as an alias.

            if (!StringHelper.Equals(resourceName, name))
                resource.Aliases.Add(name);

            return resource;
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

        private string ReplaceLastPathSegment(string input, OperationAccess access)
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

    public class ResourceAccessBundle
    {
        public List<string> Resources { get; set; } = new List<string>();
        public List<string> Access { get; set; } = new List<string>();
    }

    public class ResourcePermissions
    {
        public string Resource { get; set; }
        public List<RoleAccessBundle> Permissions { get; set; } = new List<RoleAccessBundle>();
    }
}