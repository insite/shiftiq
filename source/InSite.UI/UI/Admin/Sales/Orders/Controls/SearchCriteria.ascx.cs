using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Sales.Orders.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TOrderFilter>
    {
        public override TOrderFilter Filter
        {
            get
            {
                var filter = new TOrderFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    FullName = FullName.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                FullName.Text = value.FullName;
            }
        }

        public override void Clear()
        {
            FullName.Text = null;
        }
    }
}