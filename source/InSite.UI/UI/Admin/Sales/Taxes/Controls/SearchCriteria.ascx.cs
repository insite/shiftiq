using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Sales.Taxes.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TTaxFilter>
    {
        public override TTaxFilter Filter
        {
            get
            {
                var filter = new TTaxFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    TaxName = TaxName.Text,
                    RegionCode = RegionCode.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                TaxName.Text = value.TaxName;
                RegionCode.Value = value.RegionCode;
            }
        }

        public override void Clear()
        {
            TaxName.Text = null;
            RegionCode.Value = null;
        }
    }
}