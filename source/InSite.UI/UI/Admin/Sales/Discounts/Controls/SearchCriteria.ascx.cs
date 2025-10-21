using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Payments.Discounts.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TDiscountFilter>
    {
        public override TDiscountFilter Filter
        {
            get
            {
                var filter = new TDiscountFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    DiscountCode = DiscountCode.Text,
                    DiscountDescription = DiscountDescription.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DiscountCode.Text = value.DiscountCode;
                DiscountDescription.Text = value.DiscountDescription;
            }
        }

        public override void Clear()
        {
            DiscountCode.Text = null;
            DiscountDescription.Text = null;
        }
    }
}