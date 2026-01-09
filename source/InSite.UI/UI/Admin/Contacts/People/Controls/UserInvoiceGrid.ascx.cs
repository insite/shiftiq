using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Application.Payments.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class UserInvoiceGrid : UserControl
    {
        #region Fields

        private ReturnUrl _returnUrl;

        #endregion

        protected Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        public void LoadData(Guid userIdentifier)
        {
            UserIdentifier = userIdentifier;
            InvoiceList.EnablePaging = false;

            BindAddresses(userIdentifier);
        }

        private void BindAddresses(Guid userIdentifier)
        {
            var invoces = ServiceLocator.InvoiceSearch.GetInvoices(new InSite.Application.Invoices.Read.VInvoiceFilter()
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CustomerIdentifier = userIdentifier
            }, null, null, x=>x.Payments);

            if (invoces != null)
                NoOtherInvoices.Visible = invoces.Count <= 0;

            InvoicePanel.Visible = !NoOtherInvoices.Visible;
            InvoiceList.DataSource = invoces;
            InvoiceList.DataBind();

            InvoiceSectionTitle.Text = $"Invoices ({invoces.Count:n0})";
        }

        protected string GetInvoiceStatus(object value)
        {
            var status = (string)value;

            return status.IsEmpty()
                ? string.Empty
                : status.ToEnum<InvoiceStatusType>().GetDescription();
        }

        protected string GetLocalTime(object item)
        {
            return ((DateTimeOffset?)item).Format(CurrentSessionState.Identity.User.TimeZone, true);
        }

        protected string GetTransactionCode(object item)
        {
            var payments = (ICollection<QPayment>)item;

            var transactionId = new StringBuilder();

            if (payments != null && payments.Count > 0)
            {
                foreach (var payment in payments)
                {
                    if (payment.TransactionId.IsNotEmpty())
                        transactionId.Append($"{payment.TransactionId}<br>");
                }
            }

            return transactionId.ToString();
        }

        protected string GetViewEmailUrl()
        {
            var dataItem = (VInvoice)Page.GetDataItem();

            return new ReturnUrl($"/ui/admin/contacts/people/report?contact={UserIdentifier}#invoices")
                    .GetRedirectUrl($"/ui/admin/sales/invoices/outline?id={dataItem.InvoiceIdentifier}&customer={UserIdentifier}");
        }

        protected string GetRedirectUrl(string format, params object[] args)
        {
            var url = string.Format(format, args);

            if (_returnUrl == null)
                _returnUrl = new ReturnUrl("invoice");

            return _returnUrl.GetRedirectUrl(url);
        }
    }
}