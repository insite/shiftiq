using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Metadata.Columns.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VTableColumnFilter>
    {
        #region Search results

        protected override int SelectCount(VTableColumnFilter filter)
        {
            return VTableColumnSearch.Count(filter);
        }

        protected override IListSource SelectData(VTableColumnFilter filter)
        {
            return VTableColumnSearch.Select(filter);
        }

        #endregion
    }
}