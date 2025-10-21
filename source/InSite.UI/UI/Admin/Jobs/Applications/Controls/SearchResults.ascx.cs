using System.ComponentModel;

using InSite.Common.Web.UI;

using InSite.Persistence;

namespace InSite.Admin.Jobs.Applications.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TApplicationFilter>
    {
        protected override int SelectCount(TApplicationFilter filter)
        {
            return TApplicationSearch.Count(filter);
        }

        protected override IListSource SelectData(TApplicationFilter filter)
        {
            return TApplicationSearch.SelectSearchResults(filter);
        }
    }
}