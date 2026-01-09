using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Outlines.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/messages/messages/search";

        private Guid? MessageIdentifier => Guid.TryParse(Request.QueryString["message"], out var value) ? value : (Guid?)null;

        private string OutlineUrl => $"/ui/admin/messages/outline?message={MessageIdentifier}&tab=message";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/outline") ? $"message={MessageIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += (s, a) => { if (Page.IsValid) SendFile(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var message = MessageIdentifier.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(MessageIdentifier.Value)
                : null;

            if (message == null || message.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                null,
                $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>");

            if (message.SenderEmail.IsEmpty())
                ScreenStatus.AddMessage(AlertType.Error, Outline.ErrorSenderNotFound);

            Details.SetOutputValues(message, false);

            FileName.Text = string.Format("message-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            CancelLink.NavigateUrl = OutlineUrl;
        }

        private void SendFile()
        {
            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
            {
                var data = MessageHelper.Serialize(
                    ServiceLocator.MessageSearch.GetMessage(MessageIdentifier.Value));

                if (CompressionMode.Value == "ZIP")
                    SendZipFile(data, FileName.Text, "json");
                else
                    Response.SendFile(FileName.Text, "json", data);
            }
        }
    }
}