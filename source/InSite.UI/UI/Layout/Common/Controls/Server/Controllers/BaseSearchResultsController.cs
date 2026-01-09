using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    #region Classes (event args)

    public class SearchResultsRowCommandArgs : EventArgs
    {
        public GridViewRow Row { get; }
        public string CommandName { get; }
        public object CommandArgument { get; }
        public bool Canceled { get; set; }

        public SearchResultsRowCommandArgs(GridViewRow row, string name, object args)
        {
            Row = row;
            CommandName = name;
            CommandArgument = args;
            Canceled = false;
        }
    }

    #endregion

    public abstract class BaseSearchResultsController<TFilter> : BaseUserControl, ISearchResults where TFilter : Filter
    {
        #region Events

        public delegate void ItemCommandHandler(object sender, SearchResultsRowCommandArgs args);

        public event EventHandler Searched;
        protected void OnSearched()
        {
            Searched?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler SavingSearch;
        protected void OnSavingSearch()
        {
            SavingSearch?.Invoke(this, EventArgs.Empty);
        }

        public event BooleanValueHandler DataStateChanged;
        protected virtual void OnDataStateChanged(bool hasData)
        {
            DataStateChanged?.Invoke(this, new BooleanValueArgs(hasData));
        }

        public event ItemCommandHandler RowCommand;
        protected virtual void OnRowCommand(GridViewRow row, string name, object argument)
        {
            RowCommand?.Invoke(this, new SearchResultsRowCommandArgs(row, name, argument));
        }

        #endregion

        #region Properties

        public TFilter Filter
        {
            get { return (TFilter)ViewState[nameof(Filter)]; }
            set { ViewState[nameof(Filter)] = value; }
        }

        public abstract bool HasData
        {
            get;
        }

        public abstract DateTimeOffset? LastSearched
        {
            get;
        }

        protected virtual string InstructionText => "Click the Search button in the Criteria panel.";

        protected virtual IFinder Finder => (NamingContainer as IFinder ?? NamingContainer?.NamingContainer as IFinder);

        public virtual int RowCount { get => 0; }

        #endregion

        #region Searching

        public abstract void Search(TFilter filter, bool refreshLastSearched = false);

        public abstract void Clear(TFilter filter);

        #endregion

        #region Save & Load searches

        public virtual void SaveSearch()
        {
            SaveSearch(Filter, false);
        }

        public abstract void SaveSearch(TFilter filter, bool refreshLastSearched);

        public abstract bool LoadSearch();

        #endregion

        #region Export

        public IListSource GetExportData(int? take = null)
        {
            Filter.Paging = (take == null)
                ? null
                : Paging.SetSkipTake(0, take.Value);

            return GetExportData(Filter, false);
        }

        public virtual IListSource GetExportData(TFilter filter, bool empty)
        {
            return null;
        }

        #endregion
    }
}
