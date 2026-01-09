using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls.Navigation.Models
{
    public class NavigationRoot
    {
        public NavigationHome Home { get; set; }
        public List<NavigationCategory> Menu { get; set; }

        public NavigationRoot Clone()
        {
            return new NavigationRoot
            {
                Home = this.Home?.Clone(),
                Menu = this.Menu?.Select(x => x.Clone()).ToList()
            };
        }

        #region Default Instance

        private static readonly NavigationRoot _sidebarInstance;

        static NavigationRoot()
        {
            _sidebarInstance = LoadInstance(Path.Combine("UI", "Layout", "Admin"), new[]
            {
                "Sidebar-{0}.json",
                "Sidebar.json"
            });
        }

        private static NavigationRoot LoadInstance(string location, IEnumerable<string> filePatterns)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = Path.GetFullPath(Path.Combine(baseDir, location));

            foreach (var pattern in filePatterns)
            {
                var fileName = pattern.Format(ServiceLocator.Partition.Slug);
                var filePath = Path.Combine(dataDir, fileName);

                if (!File.Exists(filePath))
                    continue;

                try
                {
                    var json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<NavigationRoot>(json);
                }
                catch (Exception ex)
                {
                    if (ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Local)
                        AppSentry.SentryError(ex);
                    else
                        throw;
                }
            }

            return null;
        }

        public static NavigationRoot GetSidebarInstance() => _sidebarInstance?.Clone();

        #endregion
    }
}