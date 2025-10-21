using System;
using System.Data;
using System.Web.UI;

using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public abstract class BaseSearchResultsController : UserControl
    {
        #region Events

        public event EventHandler Searching;
        protected void OnSearching() =>
            Searching?.Invoke(this, EventArgs.Empty);

        public event EventHandler SavingSearch;
        protected void OnSavingSearch() =>
            SavingSearch?.Invoke(this, EventArgs.Empty);

        public event BooleanValueHandler DataStateChanged;
        protected virtual void OnDataStateChanged(bool hasData) =>
            DataStateChanged?.Invoke(this, new BooleanValueArgs(hasData));

        #endregion

        #region Properties

        public Shift.Common.Filter Filter
        {
            get { return (Shift.Common.Filter)ViewState[nameof(Filter)]; }
            set { ViewState[nameof(Filter)] = value; }
        }

        public abstract Boolean HasData
        {
            get;
        }

        protected virtual String InstructionText
        {
            get { return "Please click the <strong>Search</strong> panel, then input your criteria and click the <strong>Filter</strong> button."; }
        }

        protected virtual IFinder Finder => (NamingContainer as IFinder ?? NamingContainer?.NamingContainer as IFinder);

        public virtual int RowCount { get => 0; }

        public abstract DateTimeOffset? LastSearched { get; }

        #endregion

        #region Searching

        public abstract void Search(Shift.Common.Filter filter, bool refreshLastSearched);

        public abstract void Clear(Shift.Common.Filter filter);

        #endregion

        #region Save & Load searches

        public abstract void SaveSearch(bool refreshLastSearched);

        public abstract void SaveSearch(Shift.Common.Filter filter, bool refreshLastSearched);

        public abstract Boolean LoadSearch();

        #endregion

        #region Export

        public virtual DataTable GetAllData()
        {
            return null;
        }

        #endregion
    }
}
