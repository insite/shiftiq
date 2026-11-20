using System.Linq;

using InSite.Application.Invoices.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class TaxRegionComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var data = ServiceLocator.InvoiceSearch.GetTaxes(new TTaxFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
            });

            var items = data
                .Select(x => x.RegionCode)
                .Distinct()
                .Select(x => new ListItem { Value = x, Text = ServiceLocator.ProvinceSearch.Unabbreviate(x) })
                .OrderBy(x => x.Text);

            return new ListItemArray(items);
        }
    }
}
