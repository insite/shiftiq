using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace Shift.Sdk.UI.Navigation
{
    public static class NavigationRootFactory
    {
        public delegate string GetResourceFunc(string url);

        private static readonly string[] _filePatterns = new[]
        {
            "sidebar-{0}.json",
            "sidebar.json"
        };

        private static bool _isLoaded;
        private static NavigationRoot _instance;

        public static NavigationRoot Create(string slug, GetResourceFunc getResource)
        {
            if (!_isLoaded)
            {
                _instance = Load(slug, getResource);
                _isLoaded = true;
            }
            return _instance?.Clone();
        }

        private static NavigationRoot Load(string slug, GetResourceFunc getResource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            NavigationRoot root = null;

            foreach (var pattern in _filePatterns)
            {
                var resourceName = "Shift.Sdk.UI.Navigation." + string.Format(pattern, slug);

                if (!resourceNames.Any(x => string.Equals(x, resourceName)))
                    continue;

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        root = JsonConvert.DeserializeObject<NavigationRoot>(json);
                        break;
                    }
                }
            }

            if (root != null)
                SetLinkPermissionId(getResource, root);

            return root;
        }

        private static void SetLinkPermissionId(GetResourceFunc getResource, NavigationRoot root)
        {
            foreach (var category in root.Menu)
                SetLinkPermissionId(getResource, category.Links);
        }

        private static void SetLinkPermissionId(GetResourceFunc getResource, List<NavigationLink> links)
        {
            foreach (var link in links)
            {
                link.Resource = getResource(link.Href);

                if (link.Links != null)
                    SetLinkPermissionId(getResource, link.Links);
            }
        }
    }
}
