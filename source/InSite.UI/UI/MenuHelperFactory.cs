using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI
{
    static class MenuHelperFactory
    {
        private static MenuHelper _instance;

        public static MenuHelper Create()
        {
            if (_instance != null)
                return _instance;

            Func<string, List<MenuHelper.ActionModel>> searchActions = startsWith =>
            {
                var startsWith1 = startsWith + "/";
                var startsWith2 = "ui/" + startsWith + "/";
                return TActionSearch
                    .Search(x => x.ActionUrl.StartsWith(startsWith1) || x.ActionUrl.StartsWith(startsWith2))
                    .Select(ToModel)
                    .ToList();
            };

            Func<string, MenuHelper.ActionModel> retrieveActionByUrl = url => TActionSearch
                .Search(x => x.ActionUrl == url)
                .Select(ToModel)
                .FirstOrDefault();

            return _instance = new MenuHelper(searchActions, retrieveActionByUrl);

            MenuHelper.ActionModel ToModel(TAction action) => new MenuHelper.ActionModel
            {
                ActionList = action.ActionList,
                ActionUrl = action.ActionUrl,
                ActionNameShort = action.ActionNameShort,
                ActionName = action.ActionName,
                ActionIcon = action.ActionIcon,
                PermissionParentActionUrl = action.PermissionParent?.ActionUrl,
            };
        }
    }
}