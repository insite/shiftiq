using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Invoices.Products.Controls
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
                    ProductName = ProductName.Text,
                    ProductDescription = ProductDescription.Text,
                    ProductType = ProductType.Value,
                    IsPublished = ProductPublishedStatus.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ProductName.Text = value.ProductName;
                ProductDescription.Text = value.ProductDescription;
                ProductType.Value = value.ProductType;
                ProductPublishedStatus.ValueAsBoolean = value.IsPublished;
            }
        }

        public override void Clear()
        {
            ProductName.Text = null;
            ProductDescription.Text = null;
            ProductType.Value = null;
            ProductPublishedStatus.Value = null;
        }
    }
}