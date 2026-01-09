using System.Linq;

using InSite.Application.Invoices.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ProductTypeComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var types = ServiceLocator.InvoiceSearch
                .GetProducts(new TProductFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier })
                .Select(x => x.ProductType)
                .Distinct()
                .OrderBy(x => x);

            foreach (var type in types)
                list.Add(type, type);

            return list;
        }
    }
}