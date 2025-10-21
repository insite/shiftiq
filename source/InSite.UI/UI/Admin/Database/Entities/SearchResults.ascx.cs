using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Database.Entities
{
    public partial class SearchResults : SearchResultsGridViewController<TEntityFilter>
    {
        protected override int SelectCount(TEntityFilter filter)
        {
            return TEntitySearch.Count(filter);
        }

        protected override IListSource SelectData(TEntityFilter filter)
        {
            return new SearchResultList(TEntitySearch.Select(filter));
        }
    }
}