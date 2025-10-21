using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Occupations.Utilities.Competencies;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;
using Shift.Constant;

using AlertType = Shift.Constant.AlertType;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class CompetenciesPanel : BaseUserControl
    {
        #region Events

        public event EventHandler Updated;

        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        protected bool DocumentIsTemplate
        {
            get => (bool)ViewState[nameof(DocumentIsTemplate)];
            set => ViewState[nameof(DocumentIsTemplate)] = value;
        }

        private Guid? StandardIdentifier
        {
            get { return (Guid?)ViewState[nameof(StandardIdentifier)]; }
            set { ViewState[nameof(StandardIdentifier)] = value; }
        }

        private string AssetType
        {
            get { return (string)ViewState[nameof(AssetType)]; }
            set { ViewState[nameof(AssetType)] = value; }
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var scriptManager = ScriptManager.GetCurrent(Page);

            scriptManager.RegisterAsyncPostBackControl(SaveCompetenciesButton);
        }

        #endregion

        #region Event handlers

        private void SaveCompetenciesButton_Click(object sender, EventArgs e)
        {
            CompetenciesUpdatePanel.Update();

            if (!CompetenciesSelector.FrameworkKey.HasValue)
            {
                Status.AddMessage(AlertType.Error, Translate("Please select Framework before save."));
                return;
            }

            if (!StandardSearch.Exists(x => x.ParentStandardIdentifier == CompetenciesSelector.FrameworkKey.Value))
            {
                Status.AddMessage(AlertType.Error, Translate("Please select not empty Framework before save."));
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

            Status.AddMessage(AlertType.Success, Translate("Saved."));

            Refresh();

            OnUpdated();
        }

        private void NodeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var node = (CompetenciesNode)e.Item.FindControl("CompetenciesNode");
            node.Command += Node_Command;
        }

        private void NodeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var standard = (StandardInfo)e.Item.DataItem;

            var node = (CompetenciesNode)e.Item.FindControl("CompetenciesNode");
            node.LoadData(standard, DocumentIsTemplate);
        }

        private void Node_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                StandardContainmentStore.Delete(StandardIdentifier.Value, (Guid)e.CommandArgument);

                var items = LoadCompetencies(StandardIdentifier.Value);

                BindCompetencies(items);

                OnUpdated();
            }
            else if (e.CommandName == "DeleteAll")
            {
                var list = (IEnumerable<Guid>)e.CommandArgument;

                foreach (var item in list)
                    StandardContainmentStore.Delete(StandardIdentifier.Value, item);

                var items = LoadCompetencies(StandardIdentifier.Value);

                BindCompetencies(items);

                OnUpdated();
            }
        }

        #endregion

        #region Setting and getting input values

        public void SetAssetType(string assetType)
        {
            AssetType = assetType;

            NoContainedCompetenciesWarning.InnerHtml =
                  "<i class='fas fa-exclamation-triangle'></i> <strong> "
                  + Translate("Warning")
                  + ":</strong> "
                  + Translate("NoContainedCompetenciesWarning").Replace("$StandardType", Translate(assetType));
        }

        public void SetInputValues(Standard standard)
        {
            StandardIdentifier = standard.StandardIdentifier;
            DocumentIsTemplate = standard.IsTemplate;

            Refresh();

            CompetenciesSelector.StandardIdentifier = StandardIdentifier;

            AddCompetenciesTab.Visible = !standard.IsTemplate;
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

            NoContainedCompetenciesWarning.Visible = !hasData;

            var frameworks = items.Where(x => x.StandardType == StandardType.Framework).ToArray();

            NodeRepeater.Visible = frameworks.Length > 0;
            NodeRepeater.DataSource = frameworks;
            NodeRepeater.DataBind();
        }

        private List<StandardInfo> LoadCompetencies(Guid standardKey)
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

        private void LoadCompetencies(Dictionary<Guid, StandardInfo> accumulator, IEnumerable<StandardInfo> children)
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