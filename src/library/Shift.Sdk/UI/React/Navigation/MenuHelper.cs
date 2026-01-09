using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace Shift.Contract
{
    public class MenuHelper
    {
        public class ActionModel
        {
            public string ActionList { get; set; }
            public string ActionUrl { get; set; }
            public string ActionNameShort { get; set; }
            public string ActionName { get; set; }
            public string ActionIcon { get; set; }
            public Guid? PermissionParentActionIdentifier { get; set; }
        }

        private readonly bool _isOperator;
        private readonly Func<string, bool> _isGrantedByName;
        private readonly Func<Guid?, bool> _isGrantedById;
        private readonly Func<string, bool> _isInGroup;
        private readonly Func<string, List<ActionModel>> _searchActions;
        private readonly Func<string, ActionModel> _retrieveActionByUrl;

        public MenuHelper(
            bool isOperator,
            Func<string, bool> isGrantedByName,
            Func<Guid?, bool> isGrantedById,
            Func<string, bool> isInGroup,
            Func<string, List<ActionModel>> searchActions,
            Func<string, ActionModel> retrieveActionByUrl
            )
        {
            _isOperator = isOperator;
            _isGrantedByName = isGrantedByName;
            _isGrantedById = isGrantedById;
            _isInGroup = isInGroup;
            _searchActions = searchActions;
            _retrieveActionByUrl = retrieveActionByUrl;
        }

        public List<NavigationList> GetNavigationGroups(bool isCmds)
        {
            return isCmds
                ? GetCmdsGroups()
                : GetShiftGroups();
        }

        private List<NavigationList> GetCmdsGroups()
        {
            var items = new List<NavigationItem>();

            if (_isGrantedByName(PermissionNames.Custom_CMDS_Managers))
            {
                items.Add(new NavigationItem
                {
                    Url = Urls.AdminReportsUrl,
                    Title = "Reports",
                    Icon = "fas fa-file-chart-line"
                });
            }

            if (_isOperator
                || _isInGroup(CmdsRole.Programmers)
                || _isInGroup(CmdsRole.SystemAdministrators)
                || _isInGroup(CmdsRole.OfficeAdministrators)
                || _isInGroup(CmdsRole.FieldAdministrators)
                || _isInGroup(CmdsRole.Managers)
                || _isInGroup(CmdsRole.Supervisors)
                || _isInGroup(CmdsRole.Validators))
                items.Add(new NavigationItem
                {
                    Url = "/ui/admin/tools",
                    Title = "Tools",
                    Icon = "fas fa-tools"
                });

            if (_isGrantedByName(PermissionNames.Custom_CMDS_Users))
            {
                items.Add(new NavigationItem
                {
                    Url = "/ui/cmds/admin/users/search",
                    Title = "People",
                    Icon = "fas fa-users"
                });
            }

            if (_isGrantedByName(PermissionNames.Custom_CMDS_Administrators))
            {
                items.Add(new NavigationItem
                {
                    Url = "/ui/cmds/admin/organizations/search",
                    Title = "Organizations",
                    Icon = "fas fa-city"
                });
            }

            var general = new NavigationList
            {
                Title = "CMDS",
                MenuItems = items.ToArray()
            };

            var list = new List<NavigationList> { general };

            var actions = _searchActions("cmds")
                .Where(x => x.ActionList != null && _isGrantedById(x.PermissionParentActionIdentifier))
                .GroupBy(x => x.ActionList)
                .Select(g => new NavigationList
                {
                    Title = g.Key,
                    MenuItems = g
                        .Select(x => new NavigationItem
                        {
                            Url = $"/{x.ActionUrl}",
                            Title = x.ActionNameShort ?? x.ActionName,
                            Icon = x.ActionIcon
                        })
                        .OrderBy(x => x.Title)
                        .ToArray()
                })
                .OrderByDescending(x => x.Title)
                .ToArray();

            foreach (var action in actions)
                list.Add(action);

            var programsMenuItem = _retrieveActionByUrl("ui/admin/learning/programs/search");
            if (programsMenuItem != null && _isGrantedById(programsMenuItem.PermissionParentActionIdentifier))
            {
                var librariesMenu = list.FirstOrDefault(x => x.Title == "Management");
                if (librariesMenu != null)
                {
                    var programsMenu = librariesMenu.MenuItems.ToList();
                    programsMenu.Add(new NavigationItem
                    {
                        Url = "/ui/admin/learning/programs/search",
                        Title = "Programs",
                        Icon = "fas fa-map-marked-alt"
                    });
                    librariesMenu.MenuItems = programsMenu.ToArray();
                }
            }

            return list;
        }

        private List<NavigationList> GetShiftGroups()
        {
            var actions = _searchActions("admin");
            var groups = actions
                .Where(x =>
                    x.ActionList != null
                    && x.ActionUrl != null
                    && x.ActionUrl.StartsWith("ui/admin/")
                    && IsToolkitHome(x.ActionUrl)
                    && x.ActionUrl != "ui/admin/home"
                    && _isGrantedById(x.PermissionParentActionIdentifier)
                )
                .GroupBy(x => x.ActionList)
                .Select(g => new NavigationList
                {
                    Title = g.Key,
                    MenuItems = g
                        .Select(x => new NavigationItem
                        {
                            Url = $"/{x.ActionUrl}",
                            Title = x.ActionNameShort ?? x.ActionName,
                            Icon = x.ActionIcon,
                            PermissionActionIdentifier = x.PermissionParentActionIdentifier
                        })
                        .OrderBy(x => x.Title)
                        .ToArray()
                })
                .OrderBy(x => x.Title)
                .ToArray();

            var result = new List<NavigationList>
            {
                new NavigationList { Title = "Communication", MenuItems = GetMenuItems("Communication") },
                new NavigationList { Title = "Learning", MenuItems = GetMenuItems("Learning") },
                new NavigationList { Title = "Management", MenuItems = GetMenuItems("Management") }
            };
            return result;

            NavigationItem[] GetMenuItems(string title)
            {
                return groups.Where(x => x.Title == title).SelectMany(x => x.MenuItems).ToArray();
            }
        }

        private bool IsToolkitHome(string actionUrl)
        {
            if (actionUrl.EndsWith("/home"))
                return true;

            if (actionUrl == "ui/admin/reporting")
                return true;

            return false;
        }
    }
}
