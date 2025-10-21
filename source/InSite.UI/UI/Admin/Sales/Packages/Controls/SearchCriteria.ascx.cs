using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Sales.Packages.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TProductFilter>
    {
        public override TProductFilter Filter
        {
            get
            {
                var filter = new TProductFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ProductType = "Package",
                    ProductName = ProductName.Text,
                    ProductDescription = ProductDescription.Text,
                    ProductQuantity = ProductQuantity.ValueAsInt,
                    ProductPrice = ProductPrice.ValueAsDecimal,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ProductName.Text = value.ProductName;
                ProductDescription.Text = value.ProductDescription;
                ProductQuantity.ValueAsInt = value.ProductQuantity;
                ProductPrice.ValueAsDecimal = value.ProductPrice;
            }
        }

        public override void Clear()
        {
            ProductName.Text = null;
            ProductDescription.Text = null;
            ProductQuantity.ValueAsInt = null;
            ProductPrice.ValueAsDecimal = null;
        }
    }
}