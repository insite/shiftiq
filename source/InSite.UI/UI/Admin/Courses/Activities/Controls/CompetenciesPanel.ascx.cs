using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Courses.Activities.Controls
{
    public partial class CompetenciesPanel : BaseUserControl
    {
        #region Events

        public event EventHandler Updated;

        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private Guid? FrameworkIdentifier
        {
            get { return (Guid?)ViewState[nameof(FrameworkIdentifier)]; }
            set { ViewState[nameof(FrameworkIdentifier)] = value; }
        }

        private List<int> ItemsCount
        {
            get => (List<int>)ViewState[nameof(ItemsCount)];
            set => ViewState[nameof(ItemsCount)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NodeRepeater.ItemCreated += NodeRepeater_ItemCreated;
            NodeRepeater.ItemDataBound += NodeRepeater_ItemDataBound;

            SaveCompetenciesButton.Click += SaveCompetenciesButton_Click;
        }

        #endregion

        #region Event handlers

        private void SaveCompetenciesButton_Click(object sender, EventArgs e)
        {

        }

        private void NodeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var node = (CompetenciesNode)e.Item.FindControl("CompetenciesNode");
            node.Command += Node_Command;
        }

        private void NodeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var standard = (StandardInfo)e.Item.DataItem;

            var node = (CompetenciesNode)e.Item.FindControl("CompetenciesNode");
            node.LoadData(standard);
        }

        private void Node_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                StandardContainmentStore.Delete(FrameworkIdentifier.Value, (Guid)e.CommandArgument);

                var items = LoadCompetencies(FrameworkIdentifier.Value);

                BindCompetencies(items);

                OnUpdated();
            }
        }

        #endregion

        #region Setting and getting input values

        public void SetAssetType(string assetType)
        {
            AssetType.InnerText = assetType;
        }

        public void BindModelToControls(Guid course, Guid activity, Guid? framework)
        {
            Visible = framework.HasValue;

            if (!Visible)
                return;

            FrameworkIdentifier = framework;

            Refresh();

            CompetenciesSelector.BindModelToControls(course, activity, framework);
        }

        public void Refresh()
        {
            if (!FrameworkIdentifier.HasValue)
                return;

            var items = LoadCompetencies(FrameworkIdentifier.Value);

            BindCompetencies(items);
        }

        private void BindCompetencies(IEnumerable<StandardInfo> items)
        {
            var hasData = items != null && items.Any();

            NoCompetenciesAlert.Visible = !hasData;

            var frameworks = items.Where(x => x.StandardType == StandardType.Framework).ToArray();

            NodeRepeater.Visible = frameworks.Length > 0;
            NodeRepeater.DataSource = frameworks;
            NodeRepeater.DataBind();
        }

        private static List<StandardInfo> LoadCompetencies(Guid standardKey)
        {
            var data = StandardContainmentSearch.Bind(
                LinqExtensions1.Expr((StandardContainment x) => StandardInfo.Binder.Invoke(x.Child)).Expand(),
                x => x.ParentStandardIdentifier == standardKey
                  && x.Child.StandardType == StandardType.Competency);

            var accumulator = new Dictionary<Guid, StandardInfo>();
            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);

            var mapping = accumulator.Values
                .Where(x => x.ParentStandardIdentifier.HasValue)
                .GroupBy(x => x.ParentStandardIdentifier.Value)
                .ToDictionary(x => x.Key, x => x.AsQueryable().OrderBy(y => y.Sequence));

            IEnumerable<StandardInfo> topLevel = accumulator.Values
                .Where(x => x.StandardType == StandardType.Framework)
                .OrderBy(x => x.Title)
                .ToArray();
            IEnumerable<StandardInfo> prevLevel = topLevel;

            while (true)
            {
                var level = new List<StandardInfo>();

                foreach (var pInfo in prevLevel)
                {
                    if (!mapping.TryGetValue(pInfo.StandardIdentifier, out var children))
                        continue;

                    foreach (var cInfo in children)
                    {
                        var isCompetency = cInfo.StandardType == StandardType.Competency;
                        var isArea = cInfo.StandardType == StandardType.Area;
                        if (!isCompetency && !isArea || !isCompetency && pInfo.StandardType == StandardType.Competency)
                            continue;

                        cInfo.Parent = pInfo;
                        pInfo.Children.Add(cInfo);
                        level.Add(cInfo);
                    }
                }

                if (level.Count == 0)
                    break;

                prevLevel = level;
            }

            return topLevel.ToList();
        }

        private static void LoadCompetencies(Dictionary<Guid, StandardInfo> accumulator, IEnumerable<StandardInfo> children)
        {
            var typeFilter = new[] { StandardType.Framework, StandardType.Area, StandardType.Competency };
            var keyFilter = children
                .Where(x => x.ParentStandardIdentifier.HasValue && !accumulator.ContainsKey(x.ParentStandardIdentifier.Value))
                .Select(x => x.ParentStandardIdentifier.Value)
                .Distinct()
                .ToArray();

            if (keyFilter.Length == 0)
                return;

            var data = StandardSearch.Bind(
                LinqExtensions1.Expr((Standard x) => StandardInfo.Binder.Invoke(x)).Expand(),
                x => keyFilter.Contains(x.StandardIdentifier) && typeFilter.Contains(x.StandardType));

            if (data.Length == 0)
                return;

            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);
        }

        #endregion
    }
}