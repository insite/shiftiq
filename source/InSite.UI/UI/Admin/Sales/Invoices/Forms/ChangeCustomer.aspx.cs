using System;
using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.Sales.Invoices.Write.Commands;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Invoices.Forms
{
    public partial class ChangeCustomer : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? InvoiceID => Guid.TryParse(Request["invoice"], out var result) ? result : (Guid?)null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/sales/invoices/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var invoice = InvoiceID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value) : null;
                if (invoice == null || invoice.OrganizationIdentifier != Organization.OrganizationIdentifier)
                    RedirectToSearch();

                var title = $"Invoice to {invoice.CustomerFullName}";

                PageHelper.AutoBindHeader(this,null,title);

                SetInputValues(invoice);

                CancelButton.NavigateUrl = GetOutlineUrl(InvoiceID.Value);
            }
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(VInvoice invoice)
        {
            BillingCustomerID.Filter.GroupType = GroupTypes.Employer;
            BillingCustomerID.Filter.OrganizationIdentifier = Organization.Key;

            CustomerID.Filter.OrganizationIdentifier = Organization.Key;

            BillingCustomerID.Value = invoice.BusinessCustomerGroupIdentifier;
            CustomerID.Value = invoice.CustomerIdentifier;
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value);

            if (invoice.BusinessCustomerGroupIdentifier != BillingCustomerID.Value)
                ServiceLocator.SendCommand(new ChangeBusinessCustomer(InvoiceID.Value, BillingCustomerID.Value));

            if (invoice.CustomerIdentifier != CustomerID.Value)
                ServiceLocator.SendCommand(new ChangeInvoiceCustomer(InvoiceID.Value, CustomerID.Value.Value));

            RedirectToOutline();
        }

        #endregion

        #region Methods (redirect)

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl(InvoiceID.Value), true);

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/sales/invoices/search", true);

        private string GetOutlineUrl(Guid invoiceID) =>
            $"/ui/admin/sales/invoices/outline?id={invoiceID}";

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={InvoiceID.Value}"
                : null;
        }

        #endregion
    }
}