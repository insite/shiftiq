using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Helpers;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Messages : PortalBasePage
    {
        private class DataItem
        {
            public int Sequence { get; set; }
            public Guid LearnerId { get; set; }
            public Persistence.TEmailSearch.MyMessage Data { get; set; }
        }

        private IDictionary<Guid, ContentContainer> _contentContainers = null;
        private IDictionary<Guid, QRecipient> _mailoutsRecipients = null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MailItems.ItemCommand += MailItems_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindMail();
        }

        private void BindMail()
        {
            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            var learnerId = GetLearnerIdentifier();

            var deliveries = TEmailSearch
                .GetMyMessages(learnerId, Organization.Key)
                .Select((x, i) => new DataItem
                {
                    Sequence = i + 1,
                    LearnerId = learnerId,
                    Data = x
                })
                .Where(x => x.Data.DeliveryCompleted.HasValue)
                .ToArray();

            var mailoutIds = deliveries.Select(x => x.Data.MailoutIdentifier).ToArray();
            if (mailoutIds.IsNotEmpty())
            {
                _contentContainers = TContentSearch.Instance.GetBlocks(mailoutIds);
                _mailoutsRecipients = ServiceLocator.MessageSearch
                    .GetDeliveries(new Domain.Messages.DeliveryFilter
                    {
                        MailoutIdentifiers = mailoutIds,
                        RecipientIdentifier = learnerId
                    })
                    .ToDictionary(x => x.MailoutIdentifier);
            }

            MailItems.DataSource = deliveries;
            MailItems.DataBind();

            if (deliveries.Length == 0)
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("You have not received any messages."));
        }

        private Guid GetLearnerIdentifier() =>
            Request.QueryString["learner"].ToGuid(User.UserIdentifier);

        protected string GetDeliveryHtml()
        {
            var item = (DataItem)Page.GetDataItem();

            return $"<span class='badge bg-info'>" +
                $"{TimeZones.FormatDateOnly(item.Data.DeliveryCompleted.Value, CurrentSessionState.Identity.User.TimeZone)} " +
                $"{TimeZones.FormatTimeOnly(item.Data.DeliveryCompleted.Value, CurrentSessionState.Identity.User.TimeZone)}</span>";
        }

        protected string GetBodyHtml()
        {
            var item = (DataItem)Page.GetDataItem();

            var content = _contentContainers.GetOrDefault(item.Data.MailoutIdentifier, () => new ContentContainer());

            var email = EmailDraft.Create(
                Organization.OrganizationIdentifier,
                null,
                item.Data.SenderIdentifier,
                false
            );

            email.ContentSubject = content.Title.Text;
            if (email.ContentSubject.IsEmpty)
                email.ContentSubject.Default = item.Data.ContentSubject;

            email.ContentBody = content.Body.Text;
            if (email.ContentBody.IsEmpty)
                email.ContentBody.Default = item.Data.ContentBodyHtml;

            var recipient = GetRecipientAddress(item);
            email.Recipients.Add(recipient);

            if (item.Data.ContentVariables.IsNotEmpty())
                email.ContentVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Data.ContentVariables);

            var message = MessageHelper.BuildMessage(email, CurrentLanguage);
            var envelope = new EmailVariables(recipient.Identifier.Value, recipient.Address, email.OrganizationIdentifier, recipient.Variables);
            var body = MessageHelper.ReplacePlaceholdersForMailgun(Organization.Identifier, item.Data.SenderIdentifier, null, message.Body, envelope);

            return HttpUtility.HtmlEncode(body);
        }

        private EmailAddress GetRecipientAddress(DataItem item)
        {
            var recipient = _mailoutsRecipients.GetOrDefault(item.Data.MailoutIdentifier);

            EmailAddress result;

            if (recipient != null)
            {
                result = new EmailAddress(
                    recipient.UserIdentifier,
                    recipient.UserEmail,
                    recipient.PersonName,
                    recipient.PersonCode,
                    recipient.PersonLanguage);

                if (recipient.RecipientVariables != null)
                    result.Variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(recipient.RecipientVariables);
            }
            else
            {
                result = new EmailAddress(
                    item.LearnerId,
                    item.Data.RecipientEmail,
                    item.Data.RecipientName,
                    null,
                    null);

                if (item.Data.RecipientVariables != null)
                    result.Variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Data.RecipientVariables);
            }

            return result;
        }

        protected string GetVariablesHtml()
        {
            var item = (DataItem)Page.GetDataItem();
            var variables = item.Data.RecipientVariables.IsNotEmpty()
                ? ServiceLocator.Serializer.Deserialize<Dictionary<string, string>>(item.Data.RecipientVariables)
                : null;

            if (variables.IsEmpty())
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("<hr class='my-3' /><dl>");

            foreach (var v in variables)
            {
                sb.Append($"<dt>{v.Key}</dt>");
                sb.Append($"<dd>{Markdown.ToHtml(v.Value)}</dd>");
            }

            sb.Append("</dl>");
            return sb.ToString();
        }

        private void MailItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Print")
            {
                var mailoutIdentifier = Guid.Parse((string)e.CommandArgument);

                var message = TEmailSearch.GetMyMessage(GetLearnerIdentifier(), mailoutIdentifier);

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                    MarginTop = 5,
                    HeaderUrl = "",
                    HeaderSpacing = 7,
                };

                var data = HtmlConverter.HtmlToPdf(
                    GetMessage(message), settings);

                Response.SendFile($"MyMessage-{message.ContentSubject}", "pdf", data);
            }
        }

        private string GetMessage(TEmailSearch.MyMessage message)
        {
            var html = message.ContentBodyHtml;
            if (html.IsEmpty() && message.ContentBodyText.IsNotEmpty())
                html = Markdown.ToHtml(message.ContentBodyText);

            return $@"<div><strong>From: {message.SenderName} &lt;{message.SenderEmail}&gt;</strong></div>" + html;
        }
    }
}