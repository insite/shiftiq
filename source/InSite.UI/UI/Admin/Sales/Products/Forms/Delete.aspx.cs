using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Invoices.Products.Forms
{
    public partial class Delete : AdminBasePage
    {
        private const string FinderRelativePath = "/ui/admin/sales/products/search";

        private Guid ProductIdentifier => Guid.TryParse(Request["id"], out var asset) ? asset : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = FinderRelativePath;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var entity = SelectEntity();
            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect(FinderRelativePath);
                return;
            }

            PageHelper.AutoBindHeader(this, null, entity.ProductName);

            SetInputValues(entity);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.InvoiceStore.DeleteProduct(ProductIdentifier);

            HttpResponseHelper.Redirect(FinderRelativePath);
        }

        private TProduct SelectEntity() =>
            ServiceLocator.InvoiceSearch.GetProduct(ProductIdentifier);

        public void SetInputValues(TProduct entity)
        {
            var productReturnUrl = new ReturnUrl($"id={entity.ProductIdentifier}");
            var editUrl = productReturnUrl.GetRedirectUrl($"/ui/admin/sales/products/edit?id={entity.ProductIdentifier}");

            ProductName.Text = $"<a href=\"{editUrl}\">{entity.ProductName}</a>" ;
            ProductDescription.Text = !string.IsNullOrEmpty(entity.ProductDescription) ? entity.ProductDescription.Replace("\n", "<br>") : "None";
            ProductType.Text = !string.IsNullOrEmpty(entity.ProductType) ? entity.ProductType : "None";
        }
    }
}