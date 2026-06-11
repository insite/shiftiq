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

        public static Dictionary<Guid, ActionNode> PermissionNodesByIdentifier { get; set; }

        public static void Initialize(ActionNode[] actions, ActionTree navigation, ActionTree permission)
        {
            ActionsByIdentifier = actions.ToDictionary(x => x.Identifier, x => x);
            ActionsByUrl = actions.Where(x => x.Url != null).ToDictionary(x => x.Url, x => x);

            NavigationTree = navigation;
            PermissionTree = permission;

            PermissionNodesByIdentifier = PermissionTree.Flatten()
                .Select(x => x.Node)
                .ToList()
                .ToDictionary(x => x.Identifier);

            if (!PermissionNodesByIdentifier.ContainsKey(Guid.Parse("c22bd56b-2cc7-4688-93a7-20842961cec2")))
                throw new InvalidOperationException("The permission hierarchy is expected to include an entry for Admin/Events/Classes. (This is a node in the tree that has no parent and no children.)");
        }

        public static ActionNode GetAction(string url)
            => url != null && ActionsByUrl.ContainsKey(url) ? ActionsByUrl[url] : null;

        public static ActionNode GetAction(Guid id)
            => id != Guid.Empty && ActionsByIdentifier.ContainsKey(id) ? ActionsByIdentifier[id] : null;

        public static ActionNode GetActionForPermission(Guid id)
            => PermissionNodesByIdentifier.TryGetValue(id, out var action) ? action : null;
    }
}