using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace Shift.Contract.Presentation
{
    public class SiteSettings // Proposed rename = UserSessionContext
    {
        public string TimeZoneId { get; set; }
        public string OrganizationCode { get; set; }
        public string CompanyName { get; set; }
        public bool IsCmds { get; set; } = false;
        public HomeSettings Home { get; set; }
        public string UserName { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsOperator { get; set; }
        public bool IsMultiOrganization { get; set; } = false;
        public string ImpersonatorName { get; set; }
        public LinkModel MyDashboard { get; set; }
        public PermissionsModel Permissions { get; set; }
        public EnvironmentModel Environment { get; set; }
        public string StylePath { get; set; }
        public string AdminNavigationLogo { get; set; }
        public string UserHostAddress { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public List<MenuModel> NavigationGroups { get; set; }
        public List<MenuLinkModel> ShortcutGroups { get; set; }
        public List<MenuModel> AdminNavigationGroups { get; set; }
        public int PlatformSearchDownloadMaximumRows { get; set; }
        public string PartitionEmail { get; set; }
        public string CurrentLanguage { get; set; }
        public List<string> SupportedLanguages { get; set; }

        public class HomeSettings
        {
            public string Text { get; set; }
            public string Url { get; set; }
            public string Icon { get; set; }
            public string Image { get; set; }
        }

        public class LinkModel
        {
            public string Url { get; set; }
            public string Text { get; set; }
        }

        public class MenuLinkModel
        {
            public string Url { get; set; }
            public string Text { get; set; }
            public string Icon { get; set; }
        }

        public class MenuModel
        {
            public string Title { get; set; }
            public List<MenuLinkModel> MenuItems { get; set; }
        }

        public class PermissionsModel
        {
            public bool Accounts { get; set; }
            public bool Integrations { get; set; }
            public bool Settings { get; set; }
        }

        public class EnvironmentModel
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string Color { get; set; }
        }

        public static List<MenuModel> FromNavigationLists(IEnumerable<NavigationList> lists)
        {
            return lists.Select(list => new MenuModel
            {
                Title = list.Title,
                MenuItems = list.MenuItems.Select(item => new MenuLinkModel
                {
                    Url = item.Url,
                    Text = item.Title,
                    Icon = item.Icon
                }).ToList()
            }).ToList();
        }

        public static List<MenuLinkModel> FromNavigationItems(IEnumerable<NavigationItem> items)
        {
            return items.Select(item => new MenuLinkModel
            {
                Url = item.Url,
                Text = item.Title,
                Icon = item.Icon
            }).ToList();
        }
    }
}