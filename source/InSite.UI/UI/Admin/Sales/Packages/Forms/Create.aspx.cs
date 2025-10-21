using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Sales.Packages.Forms
{
    public partial class Create : AdminBasePage
    {
        public const string ProductType = "Package";

        public const string NavigateUrl = "/ui/admin/sales/packages/create";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = Search.NavigateUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = new TProduct
            {
                ProductIdentifier = UniqueIdentifier.Create()
            };

            PackageDetails.GetInputValues(entity);

            entity.OrganizationIdentifier = Organization.OrganizationIdentifier;
            entity.ProductType = ProductType;
            entity.CreatedBy = User.Identifier;
            entity.ModifiedBy = User.Identifier;

            ServiceLocator.InvoiceStore.InsertProduct(entity);

            Edit.Redirect(entity.ProductIdentifier, "saved");
        }
    }
}