using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Occupations.Utilities.Competencies;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Standards.Occupations.Controls
{
    public partial class CompetenciesPanel : BaseUserControl
    {
        #region Events

        public event EventHandler Updated;

        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private Guid? StandardIdentifier
        {
            get { return (Guid?)ViewState[nameof(StandardIdentifier)]; }
            set { ViewState[nameof(StandardIdentifier)] = value; }
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

            NodeRepeater.ItemDataBound += NodeRepeater_ItemDataBound;

            SaveCompetenciesButton.Click += SaveCompetenciesButton_Click;

            DeleteCompetenciesButton.Click += DeleteCompetenciesButton_Click;
        }

        #endregion

        #region Event handlers

        private void SaveCompetenciesButton_Click(object sender, EventArgs e)
        {
            if (!CompetenciesSelector.FrameworkKey.HasValue)
            {
                Status.AddMessage(AlertType.Error, "Please select Framework before save.");
                return;
            }

            if (!StandardSearch.Exists(x => x.ParentStandardIdentifier == CompetenciesSelector.FrameworkKey.Value))
            {
                Status.AddMessage(AlertType.Error, "Please select not empty Framework before save.");
                return;
            }

            var selectedCompetencies = CompetenciesSelector.SelectedCompetencies;

            {
                var framework = LoadCompetencies(StandardIdentifier.Value)
                    .FirstOrDefault(x => x.StandardIdentifier == CompetenciesSelector.FrameworkKey.Value);

                if (framework != null)
                {
                    var competencies = framework.EnumerateChildrenFlatten().ToList();

                    var identifiers = competencies
                        .Where(x => x.StandardType == StandardType.Competency)
                        .Select(x => x.StandardIdentifier)
                        .ToArray();

                    StandardContainmentStore.Delete(x =>
                        x.ParentStandardIdentifier == StandardIdentifier.Value &&
                        identifiers.Contains(x.ChildStandardIdentifier) &&
                        x.Child.StandardType == StandardType.Competency);
                }
            }

            var relationships = new List<StandardContainment>();

            foreach (var id in selectedCompetencies)
            {
                relationships.Add(new StandardContainment
                {
                    ParentStandardIdentifier = StandardIdentifier.Value,
                    ChildStandardIdentifier = id
                });
            }

            if (relationships.Count > 0)
                StandardContainmentStore.Insert(relationships);

            Status.AddMessage(AlertType.Success, "Saved.");

            Refresh();

            OnUpdated();
        }

        private void NodeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var standard = (StandardInfo)e.Item.DataItem;

            var node = (CompetenciesNode)e.Item.FindControl("CompetenciesNode");
            node.LoadData(standard);
        }

        private void DeleteCompetenciesButton_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedCompetencies();
            foreach (var id in selected)
                StandardContainmentStore.Delete(StandardIdentifier.Value, id);

            var items = LoadCompetencies(StandardIdentifier.Value);

            BindCompetencies(items);

            OnUpdated();
        }

        #endregion

        #region Setting and getting input values

        public void SetAssetType(string assetType)
        {
            AssetType.InnerText = assetType;
        }

        public void SetInputValues(Standard standard)
        {
            StandardIdentifier = standard.StandardIdentifier;

            Refresh();

            CompetenciesSelector.StandardIdentifier = StandardIdentifier;
        }

        public void Refresh()
        {
            if (!StandardIdentifier.HasValue)
                return;

            var items = LoadCompetencies(StandardIdentifier.Value);

            BindCompetencies(items);
        }

        private void BindCompetencies(IEnumerable<StandardInfo> items)
        {
            var hasData = items != null && items.Any();

            NoCompetenciesAlert.Visible = !hasData;

            var frameworks = items.Where(x => x.StandardType == StandardType.Framework).ToArray();

            CommandButtons.Visible = frameworks.Length > 0;

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

        private List<Guid> GetSelectedCompetencies()
        {
            var result = new List<Guid>();

            foreach (RepeaterItem item in NodeRepeater.Items)
            {
                var node = (CompetenciesNode)item.FindControl("CompetenciesNode");
                node.GetSelectedCompetencies(result);
            }

            return result;
        }

        #endregion
    }
}