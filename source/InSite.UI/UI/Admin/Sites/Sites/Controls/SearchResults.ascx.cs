using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Sites.Sites.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QSiteFilter>
    {
        protected override int SelectCount(QSiteFilter filter)
        {
            return ServiceLocator.SiteSearch.Count(filter);
        }

        protected override IListSource SelectData(QSiteFilter filter)
        {
            filter.OrderBy = "SiteDomain";

            return ServiceLocator.SiteSearch
                .Bind(x => new
                {
                    x.SiteIdentifier,
                    x.SiteTitle,
                    x.SiteDomain,
                    PageCount = x.Pages.Count()
                }, filter)
                .ToList()
                .ToSearchResult();
        }
    }
}