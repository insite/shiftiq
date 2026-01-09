using System.ComponentModel;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Invoices.Products.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TProductFilter>
    {
        protected override int SelectCount(TProductFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountProducts(filter);
        }

        protected override IListSource SelectData(TProductFilter filter)
        {
            return ServiceLocator.InvoiceSearch.GetProducts(filter)
                .ToSearchResult();
        }
    }
}