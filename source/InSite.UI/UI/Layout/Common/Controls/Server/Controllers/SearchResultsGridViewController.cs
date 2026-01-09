using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public abstract class SearchResultsGridViewController<TFilter> : SearchResultsBaseGridController<TFilter> where TFilter : Filter
    {
        #region Classes

        private class ColumnInfo : IColumn
        {
            #region Properties

            public string Title => _column.HeaderText;

            public string SortExpression
            {
                get => _column.SortExpression;
                set => _column.SortExpression = value;
            }

            public bool Visible
            {
                get => _column.Visible;
                set => _column.Visible = value;
            }

            #endregion

            #region Fields

            private DataControlField _column;

            #endregion

            #region Construction

            public ColumnInfo(DataControlField column)
            {
                _column = column;
            }

            #endregion
        }

        private sealed class ControlLinker : IControlLinker
        {
            #region Classes

            private class Accessor : Shift.Sdk.UI.ControlLinkerCache.Accessor
            {
                #region Constants

                public static readonly Accessor Empty = new Accessor();

                #endregion

                #region Fields

                public Func<object, ITextControl> GetInstructions { get; }
                public Func<object, DownloadButton> GetDownloadButton { get; }
                public Func<object, DropDownButton> GetDownloadDropDown { get; }
                public Func<object, GridView> GetGrid { get; }

                #endregion

                #region Construction

                private Accessor() : base(null)
                {
                    GetInstructions = Default<ITextControl>();
                    GetDownloadButton = Default<DownloadButton>();
                    GetDownloadDropDown = Default<DropDownButton>();
                    GetGrid = Default<GridView>();
                }

                public Accessor(Type t) : base(t)
                {
                    GetInstructions = Find<ITextControl>(t, "Instructions");
                    GetDownloadButton = Find<DownloadButton>(t, "DownloadButton");
                    GetDownloadDropDown = Find<DropDownButton>(t, "DownloadDropDown");
                    GetGrid = Find<GridView>(t, "Grid");
                }

                #endregion
            }

            #endregion

            #region Properties

            public ITextControl Instructions => _data.GetInstructions(_container);
            public DownloadButton DownloadButton => _data.GetDownloadButton(_container);
            public DropDownButton DownloadDropDown => _data.GetDownloadDropDown(_container);
            public GridView Grid => _data.GetGrid(_container);

            #endregion

            #region Fields

            private Accessor _data;
            private object _container;

            #endregion

            #region Construction

            public ControlLinker(object container)
            {
                _data = (Accessor)Shift.Sdk.UI.ControlLinkerCache.Get(container) ?? Accessor.Empty;
                _container = container;
            }

            #endregion
        }

        #endregion

        #region Fields

        private ControlLinker _linker;

        #endregion

        #region Construction

        public SearchResultsGridViewController()
        {
            _linker = new ControlLinker(this);
        }

        #endregion

        #region Properties

        protected override int GetRowCount() => _linker.Grid == null
            ? 0
            : IsFinder && !_linker.Grid.Visible
                ? 0
                : _linker.Grid.AllowCustomPaging
                    ? _linker.Grid.VirtualItemCount
                    : _linker.Grid.Rows.Count;

        public GridView GridControl => _linker.Grid;

        public override bool HasData => GetRowCount() > 0;

        protected override int GridPageIndex
        {
            get => _linker.Grid.PageIndex;
            set => _linker.Grid.PageIndex = value;
        }

        protected override int GridTotalItemCount
        {
            get => _linker.Grid.VirtualItemCount;
            set => _linker.Grid.VirtualItemCount = value;
        }

        protected override bool GridPagerVisible
        {
            get => _linker.Grid.PagerSettings.Visible;
            set => _linker.Grid.PagerSettings.Visible = value;
        }

        public override object GridDataSource
        {
            get => _linker.Grid.DataSource;
            set => _linker.Grid.DataSource = value;
        }

        protected override bool GridVisible
        {
            get => _linker.Grid.Visible;
            set => _linker.Grid.Visible = value;
        }

        protected override IControlLinker Linker => _linker;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            _linker.Grid.DataBound += Grid_DataBound;
            _linker.Grid.RowCommand += Grid_RowCommand;
            _linker.Grid.PageIndexChanging += Grid_PageIndexChanging;
            _linker.Grid.PreRender += Grid_PreRender;
            _linker.Grid.PageSize = PageSize;

            if (_linker.Grid is Grid inSiteGrid)
                inSiteGrid.AutoBinding = false;

            base.OnInit(e);
        }

        #endregion

        #region Event handlers

        protected virtual void Grid_DataBound(object sender, EventArgs e)
        {
            SaveSearch(Filter, RefreshLastSearched);
        }

        private void Grid_RowCommand(object source, GridViewCommandEventArgs e)
        {
            if (e.CommandName.IsEmpty())
                return;

            var row = GridViewExtensions.GetRow(e);
            if (row != null)
                OnRowCommand(row, e.CommandName, e.CommandArgument);
        }

        private void Grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _linker.Grid.PageIndex = e.NewPageIndex;

            Update = UpdateType.UpdatePage;
        }

        private void Grid_PreRender(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            GridViewRow pagerRow = gv.BottomPagerRow;

            if (pagerRow != null && pagerRow.Visible == false)
                pagerRow.Visible = true;
        }

        #endregion

        #region Searching

        protected override void GridDataBind() =>
            _linker.Grid.DataBind();

        protected override IEnumerable<IColumn> GetColumns() =>
            _linker.Grid.Columns.Cast<DataControlField>().Select(x => new ColumnInfo(x)).ToArray();

        #endregion

        #region Helpers

        public Guid[] GetDataKeys(GridViewRow row)
        {
            var keys = _linker.Grid.DataKeyNames;
            var result = new Guid[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                result[i] = (Guid)_linker.Grid.DataKeys[row.RowIndex][i];

            return result;
        }

        protected virtual ITemplate CreatePaginationTemplate() =>
            new GridPagination.Template();

        #endregion
    }
}