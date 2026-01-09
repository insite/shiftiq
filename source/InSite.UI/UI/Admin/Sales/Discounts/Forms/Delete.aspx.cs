using System;

using InSite.Application.Payments.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Payments.Discounts.Forms
{
    public partial class Delete : AdminBasePage
    {
        private const string FinderRelativePath = "/ui/admin/sales/discounts/search";

        private string Code => Request["code"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelLink.NavigateUrl = FinderRelativePath;
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

            PageHelper.AutoBindHeader(this, null, entity.DiscountCode);

            SetInputValues(entity);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.PaymentStore.DeleteDiscount(Code);

            HttpResponseHelper.Redirect(FinderRelativePath);
        }
        public void SetInputValues(TDiscount entity)
        {
            var discountsReturnUrl = new ReturnUrl($"code={entity.DiscountCode}");
            var editUrl = discountsReturnUrl.GetRedirectUrl($"/ui/admin/sales/discounts/edit?code={entity.DiscountCode}");

            DiscountCode.Text = $"<a href=\"{editUrl}\">{entity.DiscountCode}</a>";
            DiscountPercent.Text = $"{(entity.DiscountPercent/100):p2}";
            DiscountDescription.Text = !string.IsNullOrEmpty(entity.DiscountDescription) ? entity.DiscountDescription.Replace("\n", "<br>") : "None";
        }

        private TDiscount SelectEntity() =>
            ServiceLocator.PaymentSearch.GetDiscount(Code);
    }
}