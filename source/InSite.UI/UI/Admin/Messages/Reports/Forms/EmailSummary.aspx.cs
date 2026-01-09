using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Messages.Reports.Forms
{
    public partial class EmailSummary : AdminBasePage, IHasParentLinkParameters
    {
        private const string HomeUrl = "/ui/admin/messages/home";

        private Guid EmailIdentifier => Guid.TryParse(Request.QueryString["email"], out var value) ? value : Guid.Empty;

        private Guid? MessageIdentifier
        {
            get => (Guid?)ViewState[nameof(MessageIdentifier)];
            set => ViewState[nameof(MessageIdentifier)] = value;
        }

        protected string EmailContent
        {
            get => (string)ViewState[nameof(EmailContent)];
            set => ViewState[nameof(EmailContent)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = GetReturnUrl().IfNullOrEmpty("/ui/admin/messages/outline?message=" + MessageIdentifier + "&tab=mailout");
        }

        private void Open()
        {
            var email = TEmailSearch.Select(EmailIdentifier);
            if (email == null)
                HttpResponseHelper.Redirect(HomeUrl);

            MessageIdentifier = email.MessageIdentifier;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: email.ContentSubject);

            LoadRecipients(email.MailoutIdentifier);

            var timezone = User.TimeZone;

            var sender = TSenderSearch.Select(email.SenderIdentifier);
            if (sender != null)
                MessageSenderOutput.InnerText = $"{sender.SenderName} <{sender.SenderEmail}>";

            MessageNameField.Visible = false;
            MessageSubjectOutput.InnerText = email.ContentSubject;
            EmailContent = email.ContentBodyHtml;
            MessageIdOutput.InnerText = email.MessageIdentifier?.ToString();
            MessageStatusOutput.InnerText = email.MailoutStatus;
            MessageCodeOutput.InnerText = email.MailoutStatusCode;

            DeliveryTimeOutput.InnerText = email.MailoutScheduled.Format(timezone);

            var successDeliveriesCount = EmailSearch.Count(new EmailFilter { EmailIdentifier = email.MailoutIdentifier, StatusCode = "OK" });
            var failedDeliveriesCount = EmailSearch.Count(new EmailFilter { EmailIdentifier = email.MailoutIdentifier, StatusCode = "Forbidden" });

            FailedDeliveriesCount.InnerText = failedDeliveriesCount.ToString("n0");
            FailedDeliveriesSection.Visible = failedDeliveriesCount > 0;

            SuccessfulDeliveriesCount.InnerText = successDeliveriesCount.ToString("n0");
            SuccessfulDeliveriesSection.Visible = successDeliveriesCount > 0;
        }

        private void LoadRecipients(Guid deliveryThumbprint)
        {
            FailedDeliveriesGrid.LoadData(deliveryThumbprint);
            SuccessfulDeliveriesGrid.LoadData(deliveryThumbprint);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"message={MessageIdentifier}&tab=mailout"
                : null;
        }
    }
}