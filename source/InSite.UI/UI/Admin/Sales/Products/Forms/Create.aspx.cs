using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Invoices.Products.Forms
{
    public partial class Create : AdminBasePage
    {
        protected string FinderRelativePath => "/ui/admin/sales/products/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
                CancelButton.NavigateUrl = FinderRelativePath;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new TProduct
            {
                ProductIdentifier = UniqueIdentifier.Create()
            };

            ProductDetails.GetInputValues(entity);

            entity.OrganizationIdentifier = Organization.OrganizationIdentifier;
            entity.CreatedBy = User.Identifier;
            entity.ModifiedBy = User.Identifier;

            ServiceLocator.InvoiceStore.InsertProduct(entity);

            HttpResponseHelper.Redirect($"/ui/admin/sales/products/edit?id={entity.ProductIdentifier}&status=saved");
        }
    }
}