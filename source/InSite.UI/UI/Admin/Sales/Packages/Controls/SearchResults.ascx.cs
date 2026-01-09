using System.ComponentModel;
using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Sales.Packages.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TProductFilter>
    {
        protected override int SelectCount(TProductFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountProducts(filter);
        }

        protected override IListSource SelectData(TProductFilter filter)
        {
            return ServiceLocator.InvoiceSearch.GetProducts(filter).ToSearchResult();
        }

        protected string GetEditUrl()
        {
            var dataItem = (TProduct)Page.GetDataItem();
            return Forms.Edit.GetNavigateUrl(dataItem.ProductIdentifier);
        }
    }
}