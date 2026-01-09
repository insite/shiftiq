using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Courses.Activities.Controls
{
    public partial class CompetenciesSelector : BaseUserControl
    {
        #region Classes

        private class StandardInfo
        {
            public Guid Key { get; set; }
            public Guid ParentKey { get; set; }
            public int AssetNumber { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }
            public string Label { get; set; }
        }

        [Serializable]
        private class DataItem : CompetenciesSelectorRepeater.IInputItem
        {
            #region Properties

            public Guid Key { get; }

            public int Asset => _asset;

            public string Code => _code;

            public string Title => _title;

            public string TypeIcon { get => _typeIcon; set => _typeIcon = value; }

            public string TypeName { get => _typeName; set => _typeName = value; }

            public bool IsSelected
            {
                get
                {
                    if (IsCompetency)
                        return _isSelected || HasChildren && Children.Any(x => x.IsSelected);
                    else
                        return _parent != null && _parent._isSelected
                            || _isSelected
                            || HasChildren && Children.All(x => x.IsSelected);
                }
                set
                {
                    _isSelected = value;
                }
            }

            public bool IsCompetency { get; }

            public bool HasChildren => _children.Count > 0;

            public IReadOnlyList<DataItem> Children => _children;

            IEnumerable<CompetenciesSelectorRepeater.IInputItem> CompetenciesSelectorRepeater.IInputItem.Children =>
                _children.Cast<CompetenciesSelectorRepeater.IInputItem>();

            #endregion

            #region Fields

            [NonSerialized]
            private int _asset;

            [NonSerialized]
            private string _title;

            [NonSerialized]
            private string _code;

            [NonSerialized]
            private string _typeIcon;

            [NonSerialized]
            private string _typeName;

            private bool _isSelected;

            private DataItem _parent;

            private List<DataItem> _children;

            #endregion

            #region Construction

            private DataItem()
            {
                _children = new List<DataItem>();
            }

            private DataItem(StandardInfo info)
                : this()
            {
                Key = info.Key;
                _asset = info.AssetNumber;
                _code = info.Code;
                _title = info.Title;
                IsCompetency = info.Type == StandardType.Competency;
            }

            public DataItem(Guid key)
                : this()
            {
                Key = key;
            }

            #endregion

            #region Methods

            public DataItem Add(StandardInfo info)
            {
                var item = new DataItem(info)
                {
                    _parent = this,
                };

                _children.Add(item);

                return item;
            }

            public void RemoveEmptyNodes()
            {
                for (var i = 0; i < _children.Count; i++)
                {
                    var child = _children[i];
                    if (child.IsCompetency)
                        continue;

                    child.RemoveEmptyNodes();

                    if (!child.HasChildren)
                        _children.RemoveAt(i--);
                }
            }

            #endregion
        }

        #endregion

        #region Properties

        private Guid CourseIdentifier
        {
            get { return (Guid)ViewState[nameof(CourseIdentifier)]; }
            set { ViewState[nameof(CourseIdentifier)] = value; }
        }

        protected Guid ActivityIdentifier
        {
            get { return (Guid)ViewState[nameof(ActivityIdentifier)]; }
            set { ViewState[nameof(ActivityIdentifier)] = value; }
        }

        protected Guid? FrameworkIdentifier
        {
            get { return (Guid?)ViewState[nameof(FrameworkIdentifier)]; }
            set { ViewState[nameof(FrameworkIdentifier)] = value; }
        }

        private IReadOnlyList<DataItem> DataItems
        {
            get => (IReadOnlyList<DataItem>)ViewState[nameof(DataItems)];
            set => ViewState[nameof(DataItems)] = value;
        }

        public IEnumerable<Guid> SelectedCompetencies =>
            EnumerateFlatten(DataItems).Where(x => x.IsSelected).Select(x => x.Key);

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NodeRepeater.ItemCreated += NodeRepeater_ItemCreated;
            NodeRepeater.ItemDataBound += NodeRepeater_ItemDataBound;

            RefreshButton.Click += RefreshButton_Click;

            CommonStyle.ContentKey = typeof(CompetenciesSelector).FullName;
            CommonScript.ContentKey = typeof(CompetenciesSelector).FullName;
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

            var standard = (Controls.StandardInfo)e.Item.DataItem;

            var node = (CompetenciesNode)e.Item.FindControl("CompetenciesNode");
            node.LoadData(standard);
        }

        private void Node_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                Course2Store.DeleteActivityCompetency(CourseIdentifier, ActivityIdentifier, (Guid)e.CommandArgument);
                BindModelToControls(CourseIdentifier, ActivityIdentifier, FrameworkIdentifier);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DataItems.IsNotEmpty())
                ReadData(Repeater, DataItems);
        }

        public void BindModelToControls(Guid course, Guid activity, Guid? framework)
        {
            CourseIdentifier = course;
            ActivityIdentifier = activity;
            FrameworkIdentifier = framework;

            LoadData(ActivityIdentifier);
        }

        #endregion

        #region Methods (data binding)

        private static void ReadData(CompetenciesSelectorRepeater repeater, IReadOnlyList<DataItem> dataItems)
        {
            foreach (var item in repeater.EnumerateItems())
            {
                var dataItem = dataItems[item.Index];

                dataItem.IsSelected = item.IsSelected;

                if (dataItem.HasChildren)
                    ReadData(item.InnerRepeater, dataItem.Children);
            }
        }

        private void LoadData(Guid activity)
        {
            DataItems = null;

            if (FrameworkIdentifier.HasValue)
            {
                DataItems = LoadDataItems(
                    FrameworkIdentifier.Value,
                    CourseSearch.GetActivityCompetencies(activity));
            }

            Repeater.Visible = DataItems.IsNotEmpty();
            Repeater.LoadData(DataItems);

            BindSelectedCompetencies(FrameworkIdentifier);
        }

        private void BindSelectedCompetencies(Guid? framework)
        {
            if (framework == null)
                return;

            var items = LoadCompetencies(ActivityIdentifier, framework.Value);

            var hasData = items != null && items.Any();

            NoCompetenciesAlert.Visible = !hasData;

            var frameworks = items.Where(x => x.StandardType == StandardType.Framework).ToArray();

            NodeRepeater.Visible = frameworks.Length > 0;
            NodeRepeater.DataSource = frameworks;
            NodeRepeater.DataBind();
        }

        private static IReadOnlyList<DataItem> LoadDataItems(Guid frameworkKey, IEnumerable<Guid> selected)
        {
            var root = new DataItem(frameworkKey);
            var selectedHashSet = new HashSet<Guid>(selected);
            var classifications = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier);

            BindModelToControls(new[] { root }, selectedHashSet, classifications);

            root.RemoveEmptyNodes();

            return root.Children;
        }

        private static void BindModelToControls(IEnumerable<DataItem> parents, HashSet<Guid> selected, string[] classifications)
        {
            var keyFilter = parents.Select(x => x.Key).ToArray();
            var typeFilter = new[] { StandardType.Area, StandardType.Competency };

            var data = StandardSearch.Bind(
                x => new StandardInfo
                {
                    Key = x.StandardIdentifier,
                    ParentKey = x.ParentStandardIdentifier.Value,
                    AssetNumber = x.AssetNumber,
                    Type = x.StandardType,
                    Title = x.ContentTitle,
                    Label = x.StandardLabel,
                    Code = x.Code
                },
                x => keyFilter.Contains(x.ParentStandardIdentifier.Value) && typeFilter.Contains(x.StandardType),
                null, "ParentStandardIdentifier,Sequence,ContentTitle");

            if (data.Length == 0)
                return;

            DataItem parent = null;
            var items = new List<DataItem>(data.Length);

            foreach (var info in data)
            {
                if (parent == null || parent.Key != info.ParentKey)
                    parent = parents.FirstOrDefault(x => x.Key == info.ParentKey);

                if (info.Type != StandardType.Competency && parent.IsCompetency)
                    continue;

                var item = parent.Add(info);
                item.IsSelected = selected.Contains(item.Key);
                item.TypeName = info.Label.IfNullOrEmpty(info.Type);
                item.TypeIcon = StandardSearch.GetStandardTypeIcon(info.Type);
                items.Add(item);
            }

            BindModelToControls(items, selected, classifications);
        }

        public static List<Controls.StandardInfo> LoadCompetencies(Guid activity, Guid framework)
        {
            var competencies = CourseSearch.GetActivityCompetencies(activity);

            var data = StandardSearch.Bind(
                LinqExtensions1.Expr((Standard x) => Activities.Controls.StandardInfo.Binder.Invoke(x)).Expand(),
                x => competencies.Any(y => y == x.StandardIdentifier));

            var accumulator = new Dictionary<Guid, Activities.Controls.StandardInfo>();
            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);

            var mapping = accumulator.Values
                .Where(x => x.ParentStandardIdentifier.HasValue)
                .GroupBy(x => x.ParentStandardIdentifier.Value)
                .ToDictionary(x => x.Key, x => x.AsQueryable().OrderBy(y => y.Sequence));

            IEnumerable<Activities.Controls.StandardInfo> topLevel = accumulator.Values
                .Where(x => x.StandardType == StandardType.Framework)
                .OrderBy(x => x.Title)
                .ToArray();
            IEnumerable<Activities.Controls.StandardInfo> prevLevel = topLevel;

            while (true)
            {
                var level = new List<Activities.Controls.StandardInfo>();

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

        private static void LoadCompetencies(Dictionary<Guid, Controls.StandardInfo> accumulator, IEnumerable<Controls.StandardInfo> children)
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
                LinqExtensions1.Expr((Standard x) => Activities.Controls.StandardInfo.Binder.Invoke(x)).Expand(),
                x => keyFilter.Contains(x.StandardIdentifier) && typeFilter.Contains(x.StandardType));

            if (data.Length == 0)
                return;

            foreach (var info in data)
                accumulator.Add(info.StandardIdentifier, info);

            LoadCompetencies(accumulator, data);
        }

        #endregion

        #region Event handlers

        private void FrameworkSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(ActivityIdentifier);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadData(ActivityIdentifier);
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("InnerRepeater");
            if (repeater != null)
            {
                repeater.ItemCreated += Repeater_ItemCreated;
                repeater.ItemDataBound += Repeater_ItemDataBound;
            }
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var info = (DataItem)e.Item.DataItem;

            var repeater = (Repeater)e.Item.FindControl("InnerRepeater");
            if (repeater != null)
            {
                repeater.DataSource = info.Children;
                repeater.DataBind();
            }
        }

        #endregion

        #region Methods (helpers)

        private static IEnumerable<DataItem> EnumerateFlatten(IEnumerable<DataItem> dataItems)
        {
            foreach (var item in dataItems)
            {
                yield return item;

                foreach (var innerItem in EnumerateFlatten(item.Children))
                    yield return innerItem;
            }
        }

        #endregion
    }
}