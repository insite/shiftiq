using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Jobs.Opportunities.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TOpportunityFilter>
    {
        protected override int SelectCount(TOpportunityFilter filter)
        {
            return TOpportunitySearch.Count(filter);
        }

        protected override IListSource SelectData(TOpportunityFilter filter)
        {
            filter.OrderBy = "WhenCreated DESC,WhenPublished DESC";

            return TOpportunitySearch.SelectAdminSearchResults(filter);
        }
    }
}