using System;

using InSite.Application.Payments.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Payments.Discounts.Forms
{
    public partial class Create : AdminBasePage
    {
        protected string FinderRelativePath => "/ui/admin/sales/discounts/search";

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

                CancelLink.NavigateUrl = FinderRelativePath;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new TDiscount();
            entity.DiscountIdentifier = UniqueIdentifier.Create();

            DiscountDetails.GetInputValues(entity);
            entity.OrganizationIdentifier = Organization.OrganizationIdentifier;

            if (ServiceLocator.PaymentSearch.GetDiscount(entity.DiscountCode) == null)
            {
                ServiceLocator.PaymentStore.InsertDiscount(entity);
                HttpResponseHelper.Redirect($"/ui/admin/sales/discounts/edit?code={entity.DiscountCode}&status=saved");
            }
            else
            {
                ScreenStatus.AddMessage(AlertType.Error, "Discount with this Code already exists.");
            }
        }
    }
}