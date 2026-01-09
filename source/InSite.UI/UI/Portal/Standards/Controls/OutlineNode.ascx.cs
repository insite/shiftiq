using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class OutlineNode : BaseUserControl
    {
        #region Classes

        public class ConnectionInfo
        {
            public string ConnectionType { get; set; }
            public Guid Identifier { get; set; }
            public string Type { get; set; }
            public string Code { get; set; }
            public string CodePath { get; set; }
            public string Title { get; set; }
            public int Asset { get; set; }
        }

        public class DataItem
        {
            public Guid StandardIdentifier { get; set; }
            public Guid? ParentStandardIdentifier { get; set; }
            public string Icon { get; set; }
            public string Code { get; set; }
            public string CodePath { get; set; }
            public bool IsHidden { get; set; }
            public bool IsTheory { get; set; }
            public bool IsPractical { get; set; }
            public int Sequence { get; set; }

            public string Title { get; set; }
            public List<ContentItem> Contents { get; set; }
            public SatisfactionStatus SatisfactionStatus { get; set; }
            public bool IsExpanded { get; set; }

            public bool HasChildren => Children.Count > 0;

            public List<DataItem> Parents { get; } = new List<DataItem>();
            public List<DataItem> Children { get; } = new List<DataItem>();
            public List<ConnectionInfo> Connections { get; set; }

            public static readonly Expression<Func<Standard, DataItem>> Binder = LinqExtensions1.Expr((Standard standard) => new DataItem
            {
                StandardIdentifier = standard.StandardIdentifier,
                ParentStandardIdentifier = standard.ParentStandardIdentifier,
                Icon = standard.Icon,
                Code = standard.Code,
                IsHidden = standard.IsHidden,
                IsTheory = standard.IsTheory,
                IsPractical = standard.IsPractical,
                Sequence = standard.Sequence,
            });

            public SatisfactionStatus CalculateSatisfactionStatus()
            {
                var excludes = new HashSet<Guid> { StandardIdentifier };

                return CalculateSatisfactionStatus(excludes);
            }

            private SatisfactionStatus CalculateSatisfactionStatus(HashSet<Guid> excludes)
            {
                var status = SatisfactionStatus;

                if (status == SatisfactionStatus.NotCompleted)
                {
                    status = GetNodesStatus(Children.ToArray());

                    if (status == SatisfactionStatus.NotSatisfied)
                    {
                        var parentStatus = GetNodesStatus(Parents.ToArray());
                        if (parentStatus != SatisfactionStatus.NotCompleted)
                            status = parentStatus;
                    }
                }

                if (IsPractical
                    && status != SatisfactionStatus.NotSatisfied
                    && status != SatisfactionStatus.NotCompleted
                    )
                {
                    status = SatisfactionStatus.PartiallySatisfied;
                }

                return status;

                SatisfactionStatus GetNodesStatus(DataItem[] items)
                {
                    if (items.Length == 0)
                        return SatisfactionStatus.NotCompleted;

                    var totalCount = 0;
                    var satisfiedCount = 0;
                    var notSatisfiedCount = 0;

                    foreach (var item in items)
                    {
                        if (!excludes.Add(item.StandardIdentifier))
                            continue;

                        var nodeStatus = item.CalculateSatisfactionStatus(excludes);

                        excludes.Remove(item.StandardIdentifier);

                        if (nodeStatus == SatisfactionStatus.SatisfiedDirectly)
                            satisfiedCount++;
                        else if (nodeStatus == SatisfactionStatus.NotSatisfied)
                            notSatisfiedCount++;

                        totalCount++;
                    }

                    if (satisfiedCount > 0)
                        return satisfiedCount == totalCount 
                            ? SatisfactionStatus.SatisfiedDirectly 
                            : SatisfactionStatus.PartiallySatisfied;

                    return notSatisfiedCount == totalCount
                        ? SatisfactionStatus.NotSatisfied
                        : SatisfactionStatus.NotCompleted;
                }
            }

            public IEnumerable<DataItem> EnumerateChildrenFlatten()
            {
                foreach (var item in Children)
                {
                    yield return item;

                    foreach (var innerItem in item.EnumerateChildrenFlatten())
                        yield return innerItem;
                }
            }
        }

        public class ContentItem
        {
            public string Title { get; set; }
            public string Content { get; set; }
        }

        #endregion

        #region Properties

        protected int Depth { get; set; }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemRepeater.ItemCreated += ItemRepeater_ItemCreated;
            ItemRepeater.ItemDataBound += ItemRepeater_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void ItemRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var connectionRepeater = (Repeater)e.Item.FindControl("ConnectionRepeater");
            connectionRepeater.ItemDataBound += ConnectionRepeater_ItemDataBound;
        }

        private void ItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (DataItem)e.Item.DataItem;

            var competencyContentRepeater = (Repeater)e.Item.FindControl("CompetencyContentRepeater");
            if (item.Contents.IsNotEmpty())
            {
                competencyContentRepeater.DataSource = item.Contents;
                competencyContentRepeater.DataBind();
            }

            var connections = item.Connections;

            var connectionRepeater = (Repeater)e.Item.FindControl("ConnectionRepeater");
            connectionRepeater.Visible = connections.Count > 0;
            connectionRepeater.DataSource = CreateConnectionGroups(connections);
            connectionRepeater.DataBind();

            if (item.Children.Count > 0)
            {
                var childNodes = (DynamicControl)e.Item.FindControl("ChildNodes");
                var outlineNode = (OutlineNode)childNodes.LoadControl("~/UI/Portal/Standards/Controls/OutlineNode.ascx");

                outlineNode.LoadData(item.Children, Depth + 1);
            }
        }

        private void ConnectionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Items");
            itemRepeater.DataBind();
        }

        #endregion

        #region Data binding

        private dynamic CreateConnectionGroups(List<ConnectionInfo> connections)
        {
            return connections.GroupBy(x => x.ConnectionType).Select(g =>
            {
                var gTitle = "Unknown";

                if (g.Key.Equals("References", StringComparison.OrdinalIgnoreCase))
                    gTitle = "is referenced by";
                else if (g.Key.Equals("Resembles", StringComparison.OrdinalIgnoreCase))
                    gTitle = "is similar to";
                else if (g.Key.Equals("Satisfies", StringComparison.OrdinalIgnoreCase))
                    gTitle = "is satisfied by";
                else if (g.Key.Equals("Uses", StringComparison.OrdinalIgnoreCase))
                    gTitle = "is used by";

                var dataSource = new
                {
                    Title = Translate(gTitle),
                    Items = g.Select(x => x).ToList()
                };

                dataSource.Items.Sort((a, b) =>
                {
                    if (!string.IsNullOrEmpty(a.CodePath) && string.IsNullOrEmpty(b.CodePath))
                        return 1;

                    if (string.IsNullOrEmpty(a.CodePath) && !string.IsNullOrEmpty(b.CodePath))
                        return -1;

                    if (string.IsNullOrEmpty(a.CodePath) && string.IsNullOrEmpty(b.CodePath))
                        return string.Compare(a.Title, b.Title);

                    var partsA = a.CodePath.Split(new[] { '.' });
                    var partsB = b.CodePath.Split(new[] { '.' });
                    var index = 0;

                    while (index < partsA.Length && index < partsB.Length)
                    {
                        var partA = partsA[index];
                        var partB = partsB[index];

                        var cmp = int.TryParse(partA, out var numA) && int.TryParse(partB, out var numB)
                            ? numA.CompareTo(numB)
                            : partA.CompareTo(partB);

                        if (cmp != 0)
                            return cmp;

                        index++;
                    }

                    if (index < partsA.Length)
                        return -1;

                    if (index < partsB.Length)
                        return 1;

                    return string.Compare(a.Title, b.Title);
                });

                return dataSource;
            });
        }

        public void LoadData(IEnumerable<DataItem> nodes, int depth)
        {
            Depth = depth;

            ItemRepeater.DataSource = nodes.Where(x => !x.IsHidden);
            ItemRepeater.DataBind();
        }

        protected string GetSatisfactionHtml()
        {
            var status = ((DataItem)Page.GetDataItem()).CalculateSatisfactionStatus();
            if (status == SatisfactionStatus.NotCompleted)
                return string.Empty;

            string label, icon, indicator;

            if (status == SatisfactionStatus.PartiallySatisfied)
            {
                label = Translate("Partially Satisfied");
                icon = "fa-exclamation-triangle";
                indicator = "warning";
            }
            else if (status == SatisfactionStatus.NotSatisfied)
            {
                label = Translate("Not Satisfied");
                icon = "fa-times";
                indicator = "danger";
            }
            else
            {
                label = Translate("Satisfied");
                icon = "fa-check";
                indicator = "success";
            }

            return $"<span class='stfc-status badge bg-{indicator}'><i class='far {icon} me-1'></i>{label}</span>";
        }

        #endregion
    }
}