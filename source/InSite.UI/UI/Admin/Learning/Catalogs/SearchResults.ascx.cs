using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Learning.Catalogs
{
    public partial class SearchResults : SearchResultsGridViewController<TCatalogFilter>
    {
        protected override int SelectCount(TCatalogFilter filter)
        {
            return CourseSearch.CountCatalogs(filter);
        }

        protected override IListSource SelectData(TCatalogFilter filter)
        {
            var catalogs = CourseSearch.SearchCatalogs(filter.OrganizationIdentifier, filter.CatalogName);

            return catalogs.ToSearchResult();
        }

        protected string GetStatusHtml(object item)
            => ((CatalogSearchResult)item).GetStatusHtml();
    }
}