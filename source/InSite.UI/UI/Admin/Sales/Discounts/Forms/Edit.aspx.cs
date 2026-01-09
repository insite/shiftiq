
using System;

using InSite.Application.Payments.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Payments.Discounts.Forms
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {

        protected string SearchUrl => "/ui/admin/sales/discounts/search";

        string DefaultParameters => $"code={DiscountCode}&panel=seats";

        private string DiscountCode => Request["code"] != null ? Request["code"].ToString() : string.Empty;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"{DefaultParameters}" : GetParentLinkParameters(parent, null);

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

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

            if (Request.QueryString["status"] == "saved")
                SetStatus(EditorStatus, StatusType.Saved);

            Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            var entity = SelectEntity();

            DiscountDetails.GetInputValues(entity);

            ServiceLocator.PaymentStore.UpdateDiscount(entity);

            HttpResponseHelper.Redirect(SearchUrl);
        }

        protected void Open()
        {
            var entity = SelectEntity();
            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            PageHelper.AutoBindHeader(this, null, entity.DiscountCode);

            DiscountDetails.SetInputValues(entity);

            CancelLink.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private TDiscount SelectEntity() =>
            ServiceLocator.PaymentSearch.GetDiscount(DiscountCode);
    }
}