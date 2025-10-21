using System;
using System.Web;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Messages.Reports.Forms
{
    public partial class MailoutSummary : AdminBasePage, IHasParentLinkParameters
    {
        private const string HomeUrl = "/ui/admin/messages/home";

        private string OutlineUrl =>
            "/ui/admin/messages/outline?message=" + MessageIdentifier;

        private Guid MailoutIdentifier => Guid.TryParse(Request.QueryString["mailout"], out var value) ? value : Guid.Empty;

        private Guid? MessageIdentifier
        {
            get => (Guid?)ViewState[nameof(MessageIdentifier)];
            set => ViewState[nameof(MessageIdentifier)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = OutlineUrl + "&tab=mailout";
        }

        private void Open()
        {
            var mailout = ServiceLocator.MessageSearch.FindMailout(MailoutIdentifier);
            if (mailout == null)
                HttpResponseHelper.Redirect(HomeUrl);

            if (mailout.MessageIdentifier.HasValue)
            {
                var message = ServiceLocator.MessageSearch.GetMessage(mailout.MessageIdentifier.Value);
                if (message == null)
                    HttpResponseHelper.Redirect(HomeUrl);

                MessageIdentifier = message.MessageIdentifier;

                var sender = TSenderSearch.Select(message.SenderIdentifier);
                if (sender != null)
                    MessageSenderOutput.InnerText = $"{sender.SenderName} <{sender.SenderEmail}>";

                MessageSubjectOutput.InnerText = message.MessageTitle;
                MessageNameOutput.InnerHtml = $"<a href=\"{OutlineUrl}\">{HttpUtility.HtmlEncode(message.MessageName)}</a>";
            }

            PageHelper.AutoBindHeader(Page);

            LoadRecipients(mailout.MailoutIdentifier);

            var timezone = User.TimeZone;
            var scheduledDeliveriesCount = ScheduledDeliveriesGrid.RowCount;
            var startedDeliveriesCount = ServiceLocator.MessageSearch.CountDeliveries(new Domain.Messages.DeliveryFilter { MailoutIdentifier = mailout.MailoutIdentifier, Status = "Started" });
            var successDeliveriesCount = ServiceLocator.MessageSearch.CountDeliveries(new Domain.Messages.DeliveryFilter { MailoutIdentifier = mailout.MailoutIdentifier, Status = "Succeeded" });
            var failedDeliveriesCount = ServiceLocator.MessageSearch.CountDeliveries(new Domain.Messages.DeliveryFilter { MailoutIdentifier = mailout.MailoutIdentifier, Status = "Failed" });

            MailoutScheduledOutput.InnerText = mailout.MailoutScheduled.Format(timezone);
            MailoutCompletedOutput.InnerText = mailout.MailoutCompleted.Format(timezone);

            MailoutCompletedField.Visible = mailout.MailoutCompleted.HasValue;

            MailoutCancelledOutput.InnerText = mailout.MailoutCancelled.Format(timezone);
            MailoutCancelledField.Visible = mailout.MailoutCancelled.HasValue;
            
            ScheduledDeliveriesCount.InnerText = scheduledDeliveriesCount.ToString("n0");
            ScheduledDeliveriesSection.Visible = scheduledDeliveriesCount > 0;

            StartedDeliveriesCount.InnerText = startedDeliveriesCount.ToString("n0");
            StartedDeliveriesSection.Visible = startedDeliveriesCount > 0;

            FailedDeliveriesCount.InnerText = failedDeliveriesCount.ToString("n0");
            FailedDeliveriesSection.Visible = failedDeliveriesCount > 0;

            SuccessfulDeliveriesCount.InnerText = successDeliveriesCount.ToString("n0");
            SuccessfulDeliveriesSection.Visible = successDeliveriesCount > 0;

            var body = mailout.ContentBodyText ?? mailout.ContentBodyHtml;
            MailoutContentOutput.InnerText = MessageHelper.BuildPreviewHtml(Organization.OrganizationIdentifier, mailout.SenderIdentifier, Outlines.Forms.Outline.GetSurveyFormAsset(mailout.SurveyIdentifier), body);
        }

        private void LoadRecipients(Guid deliveryThumbprint)
        {
            ScheduledDeliveriesGrid.LoadData(deliveryThumbprint);
            StartedDeliveriesGrid.LoadData(deliveryThumbprint);
            FailedDeliveriesGrid.LoadData(deliveryThumbprint);
            SuccessfulDeliveriesGrid.LoadData(deliveryThumbprint);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/outline"))
            {
                if (MessageIdentifier.HasValue)
                    return $"message={MessageIdentifier.Value}";
            }

            return null;
        }
    }
}
