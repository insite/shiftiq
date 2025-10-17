using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public interface ISearchCriteria
    {
        #region Events

        event EventHandler Searching;
        event EventHandler Clearing;

        #endregion

        #region Properties

        Filter Filter { get; set; }

        #endregion

        #region Methods

        void Clear();
        void LoadShowColumns(ISearchResults searchResults);
        void SetDefaultShowColumns();
        void SetCheckedShowColumns(Filter filter);

        #endregion
    }
}
