using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using InSite.Domain.Foundations;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class TActionSearch
    {
        private static ReadOnlyCollection<TAction> _actions;
        private static ReadOnlyCollection<string> _actionControllerTypes;
        private static ReadOnlyDictionary<Guid, TAction> _actionsById;
        private static ReadOnlyDictionary<int, TAction> _actionsByIdHashCode;
        private static ReadOnlyDictionary<string, TAction> _actionsByName;

        static TActionSearch()
        {
            Refresh();
        }

        public static void Refresh()
        {
            var actionControllerTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var actionsById = new Dictionary<Guid, TAction>();
            var actionsByIdHashCode = new Dictionary<int, TAction>();
            var actionsByName = new Dictionary<string, TAction>(StringComparer.OrdinalIgnoreCase);

            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                _actions = db.TActions.AsNoTracking().OrderBy(x => x.ActionUrl).ToList().AsReadOnly();

                foreach (var action in _actions)
                {
                    actionControllerTypes.Add(action.ActionType);
                    actionsById.Add(action.ActionIdentifier, action);

                    var hashCode = action.ActionIdentifier.GetHashCode();
                    if (!actionsByIdHashCode.ContainsKey(hashCode))
                        actionsByIdHashCode.Add(hashCode, action);

                    actionsByName.Add(action.ActionUrl, action);
                }

                foreach (var action in _actions)
                {
                    action.NavigationParent = action.NavigationParentActionIdentifier.HasValue
                        ? actionsById.GetOrDefault(action.NavigationParentActionIdentifier.Value)
                        : null;
                    action.NavigationParent?.NavigationChildren.Add(action);

                    action.PermissionParent = action.PermissionParentActionIdentifier.HasValue
                        ? actionsById.GetOrDefault(action.PermissionParentActionIdentifier.Value)
                        : null;
                    action.PermissionParent?.PermissionChildren.Add(action);
                }
            }

            _actionControllerTypes = actionControllerTypes.OrderBy(x => x).ToList().AsReadOnly();
            _actionsById = new ReadOnlyDictionary<Guid, TAction>(actionsById);
            _actionsByIdHashCode = new ReadOnlyDictionary<int, TAction>(actionsByIdHashCode);
            _actionsByName = new ReadOnlyDictionary<string, TAction>(actionsByName);
        }

        public static int Count(TActionFilter filter)
        {
            return CreateQuery(filter).Count();
        }

        private static IQueryable<TAction> CreateQuery(TActionFilter filter)
        {
            var query = _actions.AsQueryable();

            if (filter.ExcludeActionIdentifier != null)
                query = query.Where(x => x.ActionIdentifier != filter.ExcludeActionIdentifier);

            if (filter.ActionList.IsNotEmpty())
                query = query.Where(x => x.ActionList != null && x.ActionList.Contains(filter.ActionList, StringComparison.InvariantCultureIgnoreCase));

            if (filter.ActionName.IsNotEmpty())
                query = query.Where(x => x.ActionName != null && x.ActionName.Contains(filter.ActionName, StringComparison.InvariantCultureIgnoreCase));

            if (filter.ActionUrl.IsNotEmpty())
                query = query.Where(x => x.ActionUrl != null && x.ActionUrl.Contains(filter.ActionUrl, StringComparison.InvariantCultureIgnoreCase));

            if (filter.IncludeActionType.IsNotEmpty())
                query = query.Where(x => x.ActionType == filter.IncludeActionType);

            if (filter.ExcludeActionType.IsNotEmpty())
                query = query.Where(x => x.ActionType != filter.ExcludeActionType);

            if (filter.ControllerPath.IsNotEmpty())
                query = query.Where(x => x.ControllerPath != null && x.ControllerPath.Contains(filter.ControllerPath, StringComparison.InvariantCultureIgnoreCase));

            if (filter.HasHelpUrl.HasValue)
                if (filter.HasHelpUrl.Value)
                    query = query.Where(x => x.HelpUrl != null);
                else
                    query = query.Where(x => x.HelpUrl == null);

            if (filter.NavigationParentActionIdentifier.HasValue)
                query = query.Where(x => x.NavigationParentActionIdentifier == filter.NavigationParentActionIdentifier);

            if (filter.PermissionParentActionIdentifier.HasValue)
                query = query.Where(x => x.PermissionParentActionIdentifier == filter.PermissionParentActionIdentifier);

            return query;
        }

        public static bool Exists(Guid id)
        {
            return _actionsById.ContainsKey(id);
        }

        public static bool Exists(int hashCode)
        {
            return _actionsByIdHashCode.ContainsKey(hashCode);
        }

        public static bool HasChildren(Guid action)
        {
            return _actions
                .Any(x => x.NavigationParentActionIdentifier == action
                       || x.PermissionParentActionIdentifier == action);
        }

        public static TAction Get(Guid id)
        {
            return _actionsById.TryGetValue(id, out var result) ? result : null;
        }

        public static TAction Get(int hashCode)
        {
            return _actionsByIdHashCode.TryGetValue(hashCode, out var result) ? result : null;
        }

        public static TAction Get(string url)
        {
            var startIndex = url.StartsWith("/") ? 1 : 0;
            var endIndex = url.IndexOf('?');
            var name = endIndex > 0
                ? url.Substring(startIndex, endIndex - startIndex)
                : startIndex == 0
                    ? url
                    : url.Substring(startIndex);

            return _actionsByName.TryGetValue(name, out var result)
                ? result
                : _actionsByName.TryGetValue(url, out result)
                    ? result
                    : null;
        }

        public static TAction GetByControllerPath(string path)
        {
            return _actions.FirstOrDefault(x => StringHelper.Equals(x.ControllerPath, path));
        }

        public static IEnumerable<string> GetControllerTypes()
        {
            return _actionControllerTypes;
        }

        public static IEnumerable<TAction> Search(Func<TAction, bool> predicate)
        {
            return _actions.Where(predicate);
        }

        public static List<TAction> Search(TActionFilter filter)
        {
            var query = CreateQuery(filter);

            if (filter.OrderBy.IsNotEmpty())
                query = query.OrderBy(filter.OrderBy);

            return query
                .ApplyPaging(filter)
                .ToList();
        }

        public static List<TActionSearchItem> SearchResult(TActionFilter filter)
        {
            var sortExpression = nameof(TAction.ActionUrl);

            var query = CreateQuery(filter);
            var items = query
                .Select(x => new TActionSearchItem
                {
                    ActionIdentifier = x.ActionIdentifier,
                    ActionIcon = x.ActionIcon,
                    ActionList = x.ActionList,
                    ActionName = x.ActionName,
                    ActionUrl = x.ActionUrl,
                    ControllerPath = x.ControllerPath,
                    ActionType = x.ActionType,
                    HelpUrl = x.HelpUrl,
                    HelpStatus = x.HelpStatus,

                    PermissionParentName = GetActionName(x.PermissionParentActionIdentifier),
                    PermissionChildCount = x.PermissionChildren.Count,

                    NavigationParentName = GetActionName(x.NavigationParentActionIdentifier),
                    NavigationChildCount = x.NavigationChildren.Count
                })
                .OrderBy(sortExpression)
                .ApplyPaging(filter)
                .ToList();

            return items;
        }

        private static string GetActionName(Guid? actionId)
        {
            if (actionId == null)
                return "(None)";

            var action = Get(actionId.Value);
            if (action == null)
                return "(Untitled)";

            return action.ActionNameShort.IfNullOrEmpty(action.ActionName);
        }

        #region Action Trees

        public static ActionNode[] CreateActionNodes()
        {
            return ToNodes(_actions, null);
        }

        public static ActionTree CreateActionTree(ActionMapType map)
        {
            if (map == ActionMapType.Navigation)
            {
                var actions = _actions.Where(x => x.NavigationParent != null || x.NavigationChildren.Any());
                return ActionTreeBuilder.BuildTree(ToNodes(actions, x => x.NavigationParentActionIdentifier));
            }
            else
            {
                var actions = _actions.Where(x => x.PermissionParent != null || x.PermissionChildren.Any());
                return ActionTreeBuilder.BuildTree(ToNodes(actions, x => x.PermissionParentActionIdentifier));
            }
        }

        private static ActionNode[] ToNodes(IEnumerable<TAction> actions, Func<TAction, Guid?> getParent)
        {
            var nodes = actions.Select(x => ToNode(x, getParent)).ToList();

            if (nodes.Count(x => x.Parent == null) > 1)
            {
                nodes.Add(new ActionNode { Identifier = Guid.Empty, Name = "(Root)" });
                foreach (var item in nodes.Where(x => x.Identifier != Guid.Empty && x.Parent == null))
                    item.Parent = Guid.Empty;
            }

            return nodes.ToArray();
        }

        private static ActionNode ToNode(TAction action, Func<TAction, Guid?> getParent)
        {
            var node = new ActionNode
            {
                Identifier = action.ActionIdentifier,
                Permission = action.PermissionParentActionIdentifier,
                Icon = action.ActionIcon,
                List = action.ActionList,
                Name = action.ActionName,
                NameShort = action.ActionNameShort,
                Type = action.ActionType,
                Url = action.ActionUrl
            };

            if (getParent != null)
                node.Parent = getParent(action);

            return node;
        }

        #endregion
    }
}