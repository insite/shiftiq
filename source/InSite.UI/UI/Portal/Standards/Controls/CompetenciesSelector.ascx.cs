using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class CompetenciesSelector : BaseUserControl
    {
        #region Classes

        private class StandardInfo
        {
            public Guid Key { get; set; }
            public Guid ParentKey { get; set; }
            public int Sequence { get; set; }
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

            public Guid Identifier => _identifier;

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
            private Guid _identifier;

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
                _identifier = Guid.Empty;
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

        public Guid? StandardIdentifier
        {
            get { return (Guid?)ViewState[nameof(StandardIdentifier)]; }
            set { ViewState[nameof(StandardIdentifier)] = value; }
        }

        private IReadOnlyList<DataItem> DataItems
        {
            get => (IReadOnlyList<DataItem>)ViewState[nameof(DataItems)];
            set => ViewState[nameof(DataItems)] = value;
        }

        public IEnumerable<Guid> SelectedCompetencies =>
            EnumerateFlatten(DataItems).Where(x => x.IsCompetency && x.IsSelected).Select(x => x.Key);

        public Guid? FrameworkKey => FrameworkSelector.Value;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FrameworkSelector.AutoPostBack = true;
            FrameworkSelector.ValueChanged += FrameworkSelector_ValueChanged;

            RefreshButton.Click += RefreshButton_Click;

            CommonStyle.ContentKey = typeof(CompetenciesSelector).FullName;
            CommonScript.ContentKey = typeof(CompetenciesSelector).FullName;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                FrameworkSelector.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
                FrameworkSelector.Filter.StandardTypes = new[] { StandardType.Framework };
                FrameworkSelector.Value = null;

                LoadData();
            }

            if (DataItems.IsNotEmpty())
                ReadData(Repeater, DataItems);
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

        private void LoadData()
        {
            DataItems = null;

            if (FrameworkSelector.HasValue && StandardIdentifier.HasValue)
            {
                DataItems = LoadDataItems(
                    FrameworkSelector.Value.Value,
                    StandardContainmentSearch.BindCompetencyContainments(StandardIdentifier.Value, x => x.Child.StandardIdentifier));
            }

            Repeater.Visible = DataItems.IsNotEmpty();
            Repeater.LoadData(DataItems);
        }

        private IReadOnlyList<DataItem> LoadDataItems(Guid frameworkKey, IEnumerable<Guid> selected)
        {
            var root = new DataItem(frameworkKey);
            var selectedHashSet = new HashSet<Guid>(selected);
            var classifications = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier);

            LoadDataItems(new[] { root }, selectedHashSet, classifications);

            root.RemoveEmptyNodes();

            return root.Children;
        }

        private void LoadDataItems(IEnumerable<DataItem> parents, HashSet<Guid> selected, string[] classifications)
        {
            var keyFilter = parents.Select(x => x.Key).ToArray();
            var typeFilter = new[] { StandardType.Area, StandardType.Competency };

            var data = StandardSearch.Bind(
                x => new StandardInfo
                {
                    Key = x.StandardIdentifier,
                    ParentKey = x.ParentStandardIdentifier.Value,
                    Sequence = x.Sequence,
                    AssetNumber = x.AssetNumber,
                    Type = x.StandardType,
                    Title = CoreFunctions.GetContentText(x.StandardIdentifier, ContentLabel.Title, CurrentLanguage)
                            ?? CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title),
                    Label = x.StandardLabel,
                    Code = x.Code
                },
                x => keyFilter.Contains(x.ParentStandardIdentifier.Value) && typeFilter.Contains(x.StandardType),
                "ParentKey,Sequence,Title", null);

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
                item.TypeName = Translate(info.Label.IfNullOrEmpty(info.Type));
                item.TypeIcon = StandardSearch.GetStandardTypeIcon(info.Type);
                items.Add(item);
            }

            LoadDataItems(items, selected, classifications);
        }

        #endregion

        #region Event handlers

        private void FrameworkSelector_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
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
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
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