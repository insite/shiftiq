using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Messages : PortalBasePage
    {
        private class DataItem
        {
            public int Sequence { get; set; }
            public Guid MailoutIdentifier { get; set; }
            public string ContentSubject { get; set; }
            public string ContentBodyHtml { get; set; }
            public string ContentBodyText { get; set; }
            public string ContentVariables { get; set; }
            public string SenderName { get; set; }
            public string SenderEmail { get; set; }
            public string RecipientVariables { get; set; }
            public DateTimeOffset? DeliveryCompleted { get; set; }

            public string ContentBody
            {
                get
                {
                    var body = ContentBodyHtml.IsEmpty() ? Markdown.ToHtml(ContentBodyText) : ContentBodyHtml;

                    if (ContentVariables != null)
                    {
                        var variables = ServiceLocator.Serializer.Deserialize<Dictionary<string, string>>(ContentVariables);
                        foreach (var v in variables)
                            body = body.Replace("$" + v.Key, v.Value);
                    }

                    return body;
                }
            }
        }

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

            var deliveries = TEmailSearch
                .GetMyMessages(GetLearnerIdentifier(), Organization.Key)
                .Select((x, i) => new DataItem
                {
                    MailoutIdentifier = x.MailoutIdentifier,
                    ContentSubject = x.ContentSubject,
                    ContentBodyHtml = x.ContentBodyHtml,
                    ContentBodyText = x.ContentBodyText,
                    ContentVariables = x.ContentVariables,
                    SenderName = x.SenderName,
                    SenderEmail = x.SenderEmail,
                    RecipientVariables = x.RecipientVariables,
                    DeliveryCompleted = x.DeliveryCompleted,
                    Sequence = i + 1
                })
                .Where(x => x.DeliveryCompleted.HasValue)
                .ToArray();

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
                $"{TimeZones.FormatDateOnly(item.DeliveryCompleted.Value, CurrentSessionState.Identity.User.TimeZone)} " +
                $"{TimeZones.FormatTimeOnly(item.DeliveryCompleted.Value, CurrentSessionState.Identity.User.TimeZone)}</span>";
        }

        protected string GetVariablesHtml()
        {
            var item = (DataItem)Page.GetDataItem();
            var variables = item.RecipientVariables.IsNotEmpty()
                ? ServiceLocator.Serializer.Deserialize<Dictionary<string, string>>(item.RecipientVariables)
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