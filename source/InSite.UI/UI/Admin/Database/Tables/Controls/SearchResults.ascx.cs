using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Utilities.Tables.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VTableFilter>
    {
        protected override int SelectCount(VTableFilter filter)
        {
            return VTableSearch.Count(filter);
        }

        protected override IListSource SelectData(VTableFilter filter)
        {
            return new SearchResultList(VTableSearch.Select(filter));
        }
    }
}