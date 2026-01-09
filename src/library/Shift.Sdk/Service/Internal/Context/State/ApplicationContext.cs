using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Foundations
{
    public static class ApplicationContext
    {
        public static Dictionary<Guid, ActionNode> ActionsByIdentifier { get; set; }
        public static Dictionary<string, ActionNode> ActionsByUrl { get; set; }

        public static ActionTree NavigationTree { get; set; }
        public static ActionTree PermissionTree { get; set; }

        public static void Initialize(ActionNode[] actions, ActionTree navigation, ActionTree permission)
        {
            ActionsByIdentifier = actions.ToDictionary(x => x.Identifier, x => x);
            ActionsByUrl = actions.Where(x => x.Url != null).ToDictionary(x => x.Url, x => x);

            NavigationTree = navigation;
            PermissionTree = permission;
        }

        public static ActionNode GetAction(string url)
            => url != null && ActionsByUrl.ContainsKey(url) ? ActionsByUrl[url] : null;

        public static ActionNode GetAction(Guid id)
            => id != Guid.Empty && ActionsByIdentifier.ContainsKey(id) ? ActionsByIdentifier[id] : null;

        public static ActionNode GetPermission(string url)
            => PermissionTree.Flatten().FirstOrDefault(p => StringHelper.Equals(p.Node.Url, url))?.Node;

        public static ActionNode GetPermission(Guid id)
            => PermissionTree.Flatten().FirstOrDefault(p => p.Node.Identifier == id)?.Node;
    }
}