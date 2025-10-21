using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<OrganizationFilter>
    {
        protected override int SelectCount(OrganizationFilter filter)
        {
            return OrganizationSearch.Count(filter);
        }

        protected override IListSource SelectData(OrganizationFilter filter)
        {
            return OrganizationSearch.Search(filter).ToSearchResult();
        }

        protected string GetAccountStatusHtml(object o)
        {
            if (o == null)
                return string.Empty;

            var status = o.ToString();
            if (status == "Opened")
                return "<span class='badge bg-success'>Open</span>";
            if (status == "Closed")
                return "<span class='badge bg-danger'>Closed</span>";
            return status;
        }
    }
}