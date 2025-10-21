using System.Linq;

using InSite.Application.Invoices.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ProductComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var products = ServiceLocator.InvoiceSearch
                .GetProducts(new TProductFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier })
                .OrderBy(x => x.ProductName);

            foreach (var product in products)
                list.Add(product.ProductIdentifier.ToString(), product.ProductName);

            return list;
        }
    }
}