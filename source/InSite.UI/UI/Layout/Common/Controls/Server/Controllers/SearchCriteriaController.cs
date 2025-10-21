using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public abstract class SearchCriteriaController<TFilter> : BaseUserControl, ISearchCriteria where TFilter : Filter
    {
        #region Classes

        private sealed class ControlLinker
        {
            #region Classes

            private class Accessor : ControlLinkerCache.Accessor
            {
                #region Constants

                public static readonly Accessor Empty = new Accessor();

                #endregion

                #region Fields

                public Func<object, IButtonControl> GetClearButton { get; }
                public Func<object, IButtonControl> GetSearchButton { get; }
                public Func<object, MultiComboBox> GetShowColumns { get; }
                public Func<object, BaseReportManager> GetFilterManager { get; }

                #endregion

                #region Construction

                private Accessor() : base(null)
                {
                    GetClearButton = Default<IButtonControl>();
                    GetSearchButton = Default<IButtonControl>();
                    GetShowColumns = Default<MultiComboBox>();
                    GetFilterManager = Default<BaseReportManager>();
                }

                public Accessor(Type t) : base(t)
                {
                    GetClearButton = Find<IButtonControl>(t, "ClearButton");
                    GetSearchButton = Find<IButtonControl>(t, "SearchButton");
                    GetShowColumns = Find<MultiComboBox>(t, "ShowColumns");
                    GetFilterManager = Find<BaseReportManager>(t, "FilterManager");
                }

                #endregion
            }

            #endregion

            #region Properties

            public IButtonControl ClearButton => _data.GetClearButton(_container);
            public IButtonControl SearchButton => _data.GetSearchButton(_container);
            public MultiComboBox ShowColumns => _data.GetShowColumns(_container);
            public BaseReportManager FilterManager => _data.GetFilterManager(_container);

            #endregion

            #region Fields

            private Accessor _data;
            private object _container;

            #endregion

            #region Construction

            public ControlLinker(object container)
            {
                _data = (Accessor)ControlLinkerCache.Get(container) ?? Accessor.Empty;
                _container = container;
            }

            #endregion
        }

        #endregion

        #region Events

        public event EventHandler Searching;

        protected virtual void OnSearching()
        {
            Searching?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Clearing;

        protected void OnClearing()
        {
            Clearing?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Fields

        private ControlLinker _linker;

        #endregion

        #region Properties

        public abstract TFilter Filter { get; set; }

        Filter ISearchCriteria.Filter { get => Filter; set => Filter = (TFilter)value; }

        protected virtual string[] DefaultShowColumns => null;

        protected virtual bool AllowAddScriptForSearchOnEnter => true;

        public bool DisableSearchOnEnterKey
        {
            get => (bool?)ViewState[nameof(DisableSearchOnEnterKey)] ?? false;
            set => ViewState[nameof(DisableSearchOnEnterKey)] = value;
        }

        protected virtual bool EnableSearchValidation => false;

        #endregion

        #region Construction

        public SearchCriteriaController()
        {
            _linker = new ControlLinker(this);
        }

        #endregion

        #region Initialization, Loading and PreRender

        protected override void OnInit(EventArgs e)
        {
            if (_linker.SearchButton != null)
                _linker.SearchButton.Click += SearchButton_Click;

            if (_linker.ClearButton != null)
                _linker.ClearButton.Click += ClearButton_Click;

            if (_linker.ShowColumns != null)
            {
                _linker.ShowColumns.Multiple.ActionsBox = true;

                LocalizeShowColumnsComboBox();
            }

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_linker.SearchButton != null)
            {
                AddScriptForSearchOnEnter();
                _linker.SearchButton.CausesValidation = EnableSearchValidation;
            }
        }

        #endregion

        #region Event handlers

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (EnableSearchValidation && !Page.IsValid)
                return;

            OnSearching();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            OnClearing();
        }

        #endregion

        #region Public methods

        public abstract void Clear();

        public virtual void LoadShowColumns(BaseSearchResultsController<TFilter> searchResults)
        {
            if (_linker.ShowColumns == null)
                return;

            var selectedColumns = IsPostBack ? GetCheckedShowColumns() : null;
            var columns = searchResults is SearchResultsBaseGridController<TFilter> gridController
                ? gridController.GetColumnsSelectorDataSource()
                : null;
            var items = columns == null
                ? Enumerable.Empty<Shift.Common.ListItem>()
                : columns.Select(name => new Shift.Common.ListItem { Text = name, Value = name.ToUpperInvariant() });

            _linker.ShowColumns.LoadItems(items);

            LocalizeShowColumnsItems();

            SetCheckedShowColumns(selectedColumns);
        }

        void ISearchCriteria.LoadShowColumns(ISearchResults searchResults) => LoadShowColumns((BaseSearchResultsController<TFilter>)searchResults);

        public void SetDefaultShowColumns()
        {
            SetCheckedShowColumns(DefaultShowColumns);
        }

        public void SetCheckedShowColumns(TFilter filter)
        {
            var columns = (filter?.ShowColumns).IsNotEmpty()
                ? (ICollection<string>)filter.ShowColumns
                : DefaultShowColumns;

            SetCheckedShowColumns(columns);
        }

        void ISearchCriteria.SetCheckedShowColumns(Filter filter) => SetCheckedShowColumns((TFilter)filter);

        #endregion

        #region Helper methods

        public BaseReportManager GetFilterManager() => _linker.FilterManager;

        protected void LocalizeShowColumnsItems()
        {
            if (_linker.ShowColumns == null)
                return;

            foreach (var item in _linker.ShowColumns.Items)
                item.Text = Global.Translate(item.Text);
        }

        private void LocalizeShowColumnsComboBox()
        {
            _linker.ShowColumns.Multiple.SelectAllText = Global.Translate(ComboBoxMultiple.DefaultSelectAllText);
            _linker.ShowColumns.Multiple.DeselectAllText = Global.Translate(ComboBoxMultiple.DefaultDeselectAllText);
            _linker.ShowColumns.Multiple.CountPluralFormat = Global.Translate("{0} Columns Visible");
            _linker.ShowColumns.Multiple.CountAllFormat = Global.Translate("All Columns Visible");
            _linker.ShowColumns.EmptyMessage = Global.Translate("All Columns Visible");
        }

        protected void SetCheckAll(MultiComboBox cmb, string pluralName)
        {
            cmb.Multiple.ActionsBox = true;
            cmb.Multiple.SelectAllText = Global.Translate(ComboBoxMultiple.DefaultSelectAllText);
            cmb.Multiple.DeselectAllText = Global.Translate(ComboBoxMultiple.DefaultDeselectAllText);

            var localization = new Localization("FieldTitle.Text");

            localization.Identifier = "All " + pluralName;
            cmb.Multiple.CountAllFormat = Global.Translate(localization.Identifier);

            localization.Identifier = pluralName;
            cmb.Multiple.CountPluralFormat = "{0} " + Global.Translate(localization.Identifier);
        }

        private void SetCheckedShowColumns(ICollection<string> columns)
        {
            if (_linker.ShowColumns == null)
                return;

            if (columns == null)
                _linker.ShowColumns.SelectAll();
            else if (columns.Count == 0)
                _linker.ShowColumns.ClearSelection();
            else
                _linker.ShowColumns.Values = columns.Select(x => x.ToUpperInvariant());
        }

        private string[] GetCheckedShowColumns()
        {
            var columns = new List<string>();

            GetCheckedShowColumns(columns);

            return columns.Count > 0 ? columns.ToArray() : null;
        }

        protected void GetCheckedShowColumns(TFilter filter)
        {
            GetCheckedShowColumns(filter.ShowColumns);
        }

        private void GetCheckedShowColumns(ICollection<string> columns)
        {
            columns.Clear();

            if (_linker.ShowColumns != null)
                foreach (var value in _linker.ShowColumns.Values)
                    columns.Add(value);
        }

        private void AddScriptForSearchOnEnter()
        {
            if (DisableSearchOnEnterKey) return;

            const string script = @"
$('.toolbox-section').keyup(function(event) {{
    if (event.keyCode == 13) {{
        __doPostBack('{0}', '');
    }}
}});";

            var curScript = string.Format(script, ((Control)_linker.SearchButton).UniqueID);

            ScriptManager.RegisterStartupScript(Page, GetType(), "SearchButtonKeyUp", curScript, true);
        }

        #endregion
    }
}