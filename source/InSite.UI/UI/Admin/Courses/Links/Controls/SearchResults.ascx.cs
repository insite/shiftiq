using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Courses.Links.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<LtiLinkFilter>
    {
        protected override IListSource SelectData(LtiLinkFilter filter)
        {
            return LtiLinkSearch.SelectSearchResults(filter);
        }

        protected override int SelectCount(LtiLinkFilter filter)
        {
            return LtiLinkSearch.CountByLtiLinkFilter(filter);
        }
    }
}