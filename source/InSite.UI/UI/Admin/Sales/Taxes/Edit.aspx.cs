using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Sales.Taxes
{
    public partial class Edit : AdminBasePage
    {
        private string SearchUrl => "/ui/admin/sales/taxes/search";

        private Guid TaxId => Guid.TryParse(Request.QueryString["id"], out var taxId) ? taxId : Guid.Empty;

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

            Detail.GetInputValues(entity);

            ServiceLocator.InvoiceStore.UpdateTax(entity);

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

            PageHelper.AutoBindHeader(this);

            Detail.SetInputValues(entity);

            CancelLink.NavigateUrl = SearchUrl;
        }

        private TTax SelectEntity() =>
            ServiceLocator.InvoiceSearch.GetTax(TaxId);
    }
}