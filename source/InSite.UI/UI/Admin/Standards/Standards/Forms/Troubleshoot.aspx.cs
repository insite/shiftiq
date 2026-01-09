using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Standards.Standards.Forms
{
    public partial class Troubleshoot : AdminBasePage
    {
        [Serializable]
        private class StandardInfo
        {
            public Guid OrganizationId { get; set; }
            public Guid StandardId { get; set; }
            public Guid? ParentId { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
            public int Number { get; set; }

            public bool AllowView => OrganizationId == Organization.Identifier;
            public string EditLink => AllowView ? $"/ui/admin/standards/edit?id={StandardId}" : null;
            public OrganizationState Organization => ServiceLocator.OrganizationSearch.GetModel(OrganizationId);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OrphanGrid.DataBinding += OrphanGrid_DataBinding;

            InvalidHierarchyGrid.DataBinding += InvalidHierarchyGrid_DataBinding;
            InvalidHierarchyGrid.RowDataBound += InvalidHierarchyGrid_RowDataBound;

            HierarchySizesGrid.DataBinding += HierarchySizesGrid_DataBinding;

            ShowHierarchySizesInvalidOnly.AutoPostBack = true;
            ShowHierarchySizesInvalidOnly.CheckedChanged += (x, y) => BindHierarchySizesGrid();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            LoadData();
        }

        private void LoadData()
        {
            var allStandards = StandardSearch
                .Bind(
                    x => new StandardInfo
                    {
                        OrganizationId = x.OrganizationIdentifier,
                        StandardId = x.StandardIdentifier,
                        ParentId = x.ParentStandardIdentifier,
                        Type = x.StandardType,
                        Title = x.ContentTitle,
                        Number = x.AssetNumber,
                    },
                    new StandardFilter())
                .ToDictionary(x => x.StandardId, x => x);
            var containments = ServiceLocator.StandardSearch.GetAllStandardContainments();
            var connections = ServiceLocator.StandardSearch.GetAllStandardConnections();

            var organizationId = Organization.Identifier;
            var standards = allStandards.Values.Where(x => x.OrganizationId == organizationId).ToArray();
            var rootStandards = standards.Where(x => !x.ParentId.HasValue).ToArray();

            var rootStandardsCount = rootStandards.Count();
            var childStandardsCount = standards.Where(x => x.ParentId.HasValue).Count();
            var containmentsCount = containments.Where(x => x.OrganizationIdentifier == organizationId).Count();

            RootStandardsCount.InnerText = rootStandardsCount.ToString("n0");
            ChildStandardsCount.InnerText = childStandardsCount.ToString("n0");
            ContainmentsCount.InnerText = containmentsCount.ToString("n0");

            LoadOrphans(
                allStandards,
                standards,
                containments.Where(x => x.OrganizationIdentifier == organizationId),
                connections.Where(x => x.OrganizationIdentifier == organizationId));

            LoadInvalidHierarchies(
                organizationId,
                allStandards,
                containments);

            LoadHierarchySizes(
                organizationId,
                allStandards,
                containments);
        }

        #region Orphan Relationships

        [Serializable]
        private class OrphanInfo
        {
            public int ConnectionSequence { get; set; }
            public string ConnectionType { get; set; }

            public Guid FromStandardId { get; set; }
            public StandardInfo FromStandard { get; set; }
            public bool HasFromStandard => FromStandard != null;

            public Guid ToStandardId { get; set; }
            public StandardInfo ToStandard { get; set; }
            public bool HasToStandard => ToStandard != null;
        }

        private OrphanInfo[] OrphanEntities
        {
            get => (OrphanInfo[])ViewState[nameof(OrphanEntities)];
            set => ViewState[nameof(OrphanEntities)] = value;
        }

        private void LoadOrphans(Dictionary<Guid, StandardInfo> allStandards, IEnumerable<StandardInfo> standards, IEnumerable<QStandardContainment> containments, IEnumerable<QStandardConnection> connections)
        {
            var standardOrphans = standards.Where(x => x.ParentId.HasValue && !allStandards.ContainsKey(x.ParentId.Value)).Select(x => new OrphanInfo
            {
                ConnectionSequence = 1,
                ConnectionType = "Structural Containment",
                FromStandardId = x.ParentId.Value,
                ToStandardId = x.StandardId
            }).ToArray();
            var containmentOphans = containments.Where(x => !allStandards.ContainsKey(x.ParentStandardIdentifier) || !allStandards.ContainsKey(x.ChildStandardIdentifier)).Select(x => new OrphanInfo
            {
                ConnectionSequence = 2,
                ConnectionType = "Functional Containment",
                FromStandardId = x.ParentStandardIdentifier,
                ToStandardId = x.ChildStandardIdentifier
            }).ToArray();
            var connectionOphans = connections.Where(x => !allStandards.ContainsKey(x.FromStandardIdentifier) || !allStandards.ContainsKey(x.ToStandardIdentifier)).Select(x => new OrphanInfo
            {
                ConnectionSequence = 3,
                ConnectionType = $"Connection ({x.ConnectionType})",
                FromStandardId = x.FromStandardIdentifier,
                ToStandardId = x.ToStandardIdentifier
            }).ToArray();

            OrphanEntities = standardOrphans.Concat(containmentOphans).Concat(connectionOphans)
                .Select(x =>
                {
                    x.FromStandard = allStandards.GetOrDefault(x.FromStandardId);
                    x.ToStandard = allStandards.GetOrDefault(x.ToStandardId);

                    return x;
                })
                .OrderBy(x => x.ConnectionSequence)
                .ThenBy(x => x.FromStandard?.Title).ThenBy(x => x.ToStandard?.Title)
                .ThenBy(x => x.FromStandard?.Number).ThenBy(x => x.ToStandard?.Number)
                .ToArray();

            var hasOrphans = OrphanEntities.Length > 0;

            OrphanNotFound.Visible = !hasOrphans;
            OrphanCount.Visible = hasOrphans;
            OrphanCount.InnerText = $"({OrphanEntities.Length:n0})";

            OrphanGrid.PageIndex = 0;
            OrphanGrid.VirtualItemCount = OrphanEntities.Length;
            OrphanGrid.DataBind();
        }

        private void OrphanGrid_DataBinding(object sender, EventArgs e)
        {
            var paging = Paging.SetPage(OrphanGrid.PageIndex + 1, OrphanGrid.PageSize);

            OrphanGrid.DataSource = OrphanEntities.ApplyPaging(paging).ToArray();
        }

        #endregion

        #region Invalid Hierarchies

        [Serializable]
        private class CycleInfo
        {
            public List<CyclePathItem> Path { get; } = new List<CyclePathItem>();
        }

        [Serializable]
        private class CyclePathItem
        {
            public Guid StandardId { get; set; }
            public StandardInfo Standard { get; set; }
            public bool HasStandard => Standard != null;
            public bool IsLoopNode { get; set; }
        }

        private CycleInfo[] InvalidEntities
        {
            get => (CycleInfo[])ViewState[nameof(InvalidEntities)];
            set => ViewState[nameof(InvalidEntities)] = value;
        }

        private void LoadInvalidHierarchies(Guid organizationId, Dictionary<Guid, StandardInfo> allStandards, IEnumerable<QStandardContainment> containments)
        {
            var (roots, relationships) = GetParentChildRelationshipsAndRoots(allStandards, containments);
            var cycles = new List<CycleInfo>();
            Guid? invalidId = null;

            GoThroughGraph(roots, id => relationships.GetOrDefault(id), state =>
            {
                if (invalidId.HasValue)
                {
                    if (state.PathContains(invalidId.Value))
                        return false;

                    invalidId = null;
                }

                if (!state.PathContains(state.Id))
                    return true;

                invalidId = state.Id;

                var info = new CycleInfo();

                foreach (var id in state.GetPath())
                {
                    info.Path.Add(new CyclePathItem
                    {
                        StandardId = id,
                        Standard = allStandards.GetOrDefault(id),
                        IsLoopNode = id == state.Id
                    });
                }

                info.Path.Add(new CyclePathItem
                {
                    StandardId = state.Id,
                    Standard = allStandards.GetOrDefault(state.Id),
                    IsLoopNode = true
                });

                cycles.Add(info);

                return false;
            });

            cycles.RemoveAll(x => !x.Path.Any(y => y.HasStandard && y.Standard.OrganizationId == organizationId));

            InvalidEntities = cycles.OrderBy(x => x.Path[0].Standard?.Title).ThenBy(x => x.Path[0].Standard?.Number).ToArray();

            var hasData = InvalidEntities.Length > 0;

            InvalidHierarchyNotFound.Visible = !hasData;
            InvalidHierarchyCount.Visible = hasData;
            InvalidHierarchyCount.InnerText = $"({InvalidEntities.Length:n0})";

            InvalidHierarchyGrid.PageIndex = 0;
            InvalidHierarchyGrid.VirtualItemCount = InvalidEntities.Length;
            InvalidHierarchyGrid.DataBind();
        }

        private void InvalidHierarchyGrid_DataBinding(object sender, EventArgs e)
        {
            var paging = Paging.SetPage(InvalidHierarchyGrid.PageIndex + 1, InvalidHierarchyGrid.PageSize);

            InvalidHierarchyGrid.DataSource = InvalidEntities.ApplyPaging(paging).ToArray();
        }

        private void InvalidHierarchyGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var info = (CycleInfo)e.Row.DataItem;

            var repeater = (Repeater)e.Row.FindControl("ItemRepeater");
            repeater.DataSource = info.Path;
            repeater.DataBind();
        }

        #endregion

        #region Hierarchy Sizes

        protected const int MaxHierarchyDepth = 6;

        [Serializable]
        private class SizeItem
        {
            public Guid StandardId { get; set; }
            public StandardInfo Standard { get; set; }
            public bool IsValidHierarchy { get; set; }
            public int Depth { get; set; }
            public bool HasStandard => Standard != null;
            public bool IsValidDepth => Depth <= MaxHierarchyDepth;
            public bool IsValid => IsValidHierarchy && IsValidDepth;
        }

        private SizeItem[] SizeEntities
        {
            get => (SizeItem[])ViewState[nameof(SizeEntities)];
            set => ViewState[nameof(SizeEntities)] = value;
        }

        private void LoadHierarchySizes(Guid organizationId, Dictionary<Guid, StandardInfo> allStandards, IEnumerable<QStandardContainment> containments)
        {
            var (roots, relationships) = GetParentChildRelationshipsAndRoots(
                allStandards.Values.Where(x => x.OrganizationId == organizationId).ToDictionary(x => x.StandardId, x => x),
                containments.Where(x => x.OrganizationIdentifier == organizationId));

            var data = roots.ToDictionary(x => x, x => new SizeItem
            {
                StandardId = x,
                Standard = allStandards.GetOrDefault(x),
                IsValidHierarchy = !InvalidEntities.Any(y => y.Path.Any(z => z.StandardId == x))
            });

            GoThroughGraph(roots, id => relationships.GetOrDefault(id), state =>
            {
                var item = data[state.RootId];
                if (!item.IsValidHierarchy)
                    return false;

                if (item.Depth < state.Depth)
                    item.Depth = state.Depth;

                return true;
            });

            SizeEntities = data.Values.OrderBy(x => x.Standard?.Title).ThenBy(x => x.Standard?.Number).ToArray();

            var hasData = SizeEntities.Length > 0;
            var validCount = SizeEntities.Where(x => x.IsValid).Count();
            var invalidCount = SizeEntities.Where(x => !x.IsValid).Count();

            HierarchySizesNotFound.Visible = !hasData;
            HierarchySizesCount.Visible = hasData;
            HierarchySizesCount.InnerText = $"({validCount:n0} valid, {invalidCount:n0} invalid)";

            BindHierarchySizesGrid();
        }

        private void BindHierarchySizesGrid()
        {
            HierarchySizesGrid.PageIndex = 0;
            HierarchySizesGrid.VirtualItemCount = ShowHierarchySizesInvalidOnly.Checked
                ? SizeEntities.Where(x => !x.IsValid).Count()
                : SizeEntities.Length;
            HierarchySizesGrid.DataBind();
        }

        private void HierarchySizesGrid_DataBinding(object sender, EventArgs e)
        {
            var paging = Paging.SetPage(HierarchySizesGrid.PageIndex + 1, HierarchySizesGrid.PageSize);
            var query = SizeEntities.AsQueryable();

            if (ShowHierarchySizesInvalidOnly.Checked)
                query = query.Where(x => !x.IsValid);

            HierarchySizesGrid.DataSource = query.ApplyPaging(paging).ToArray();
        }

        #endregion

        #region Helper methods

        private interface IGraphLoopState
        {
            Guid Id { get; }
            Guid RootId { get; }
            int Depth { get; }
            bool PathContains(Guid id);
            Guid[] GetPath();
        }

        private class GraphLoopState : IGraphLoopState
        {
            public IEnumerator<Guid> Enumerator { get; set; }
            public Stack<(Guid Id, IEnumerator<Guid> Enumerator)> Stack { get; set; }
            public HashSet<Guid> PathIndex { get; set; }

            public Guid Id { get; set; }
            public Guid RootId { get; set; }
            public int Depth { get; set; }
            public Guid[] GetPath() => Stack.Reverse().Skip(1).Select(x => x.Id).ToArray();
            public bool PathContains(Guid id) => PathIndex.Contains(id);
        }

        private static void GoThroughGraph(IEnumerable<Guid> roots, Func<Guid, IEnumerable<Guid>> getChildren, Func<IGraphLoopState, bool> action)
        {
            var enumerator = roots.GetEnumerator();
            var stack = new Stack<(Guid Id, IEnumerator<Guid> Enumerator)>(new[] { (Guid.Empty, enumerator) });
            var pathIndex = new HashSet<Guid>();
            var state = new GraphLoopState
            {
                Enumerator = enumerator,
                PathIndex = pathIndex,
                Stack = stack,
                Depth = 1
            };

            while (true)
            {
                if (!enumerator.MoveNext())
                {
                    var item = stack.Pop();
                    item.Enumerator.Dispose();

                    if (stack.Count == 0)
                        break;

                    pathIndex.Remove(item.Id);
                    enumerator = stack.Peek().Enumerator;
                    state.Depth -= 1;

                    continue;
                }

                state.Id = enumerator.Current;

                if (stack.Count == 1)
                    state.RootId = state.Id;

                if (!action(state))
                    continue;

                var children = getChildren(state.Id);
                if (children == null)
                    continue;

                enumerator = children.GetEnumerator();
                stack.Push((state.Id, enumerator));
                pathIndex.Add(state.Id);
                state.Depth += 1;
            }
        }

        private static (HashSet<Guid> roots, Dictionary<Guid, List<Guid>> relationships) GetParentChildRelationshipsAndRoots(Dictionary<Guid, StandardInfo> standards, IEnumerable<QStandardContainment> containments)
        {
            var roots = standards.Keys.ToHashSet();
            var relationships = new Dictionary<Guid, List<Guid>>();

            foreach (var group in standards.Values.Where(x => x.ParentId.HasValue).GroupBy(x => x.ParentId.Value))
            {
                var list = relationships.GetOrAdd(group.Key, () => new List<Guid>());
                foreach (var child in group)
                {
                    roots.Remove(child.StandardId);
                    list.Add(child.StandardId);
                }
            }

            foreach (var group in containments.GroupBy(x => x.ParentStandardIdentifier))
            {
                var list = relationships.GetOrAdd(group.Key, () => new List<Guid>());
                foreach (var containment in group)
                {
                    roots.Remove(containment.ChildStandardIdentifier);
                    list.Add(containment.ChildStandardIdentifier);
                }
            }

            return (roots, relationships);
        }

        #endregion
    }
}