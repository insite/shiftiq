using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Messages
{
    public partial class ChangeSender : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid MessageIdentifier => Guid.TryParse(Request["message"], out var value) ? value : Guid.Empty;

        private const string SearchUrl = "/ui/admin/messages/messages/search";

        string DefaultParameters => $"message={MessageIdentifier}&tab=message";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => GetParentLinkParameters(parent, null).IfNullOrEmpty($"message={MessageIdentifier}");

        IWebRoute IOverrideWebRouteParent.GetParent()
            => GetParent();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SenderInput.Changed += (s, a) => ShowWarning();

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            Open();
        }

        private void Open()
        {
            var message = ServiceLocator.MessageSearch.GetMessage(MessageIdentifier);
            if (message == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this,
                qualifier: $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>");

            if (message.SenderEmail.IsEmpty())
                ScreenStatus.AddMessage(AlertType.Error, Outlines.Forms.Outline.ErrorSenderNotFound);

            SenderInput.SenderType = "Mailgun";

            SenderInput.Value = message.SenderIdentifier;

            ShowWarning();

            CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private void Save()
        {
            ServiceLocator.SendCommand(new Application.Messages.Write.ChangeSender(MessageIdentifier, SenderInput.Value.Value));

            HttpResponseHelper.Redirect(GetParentUrl(DefaultParameters));
        }

        private void ShowWarning()
        {
            if (SenderInput.Value.HasValue)
            {
                var senderId = SenderInput.Value.Value;
                var sender = TSenderSearch.Select(senderId);
                var domains = StringHelper.Split(ServiceLocator.Partition.WhitelistDomains);

                var index = sender.SenderName.HasValue() ? sender.SystemMailbox.IndexOf("@") : -1;
                var domain = index > 0 ? sender.SystemMailbox.Substring(index + 1) : null;
                var validDomain = domain == null || StringHelper.EqualsAny(domain, domains);

                SenderWarningPanel.Visible = !validDomain;

                if (!validDomain)
                {
                    WarningEmailLiteral.Text = sender.SystemMailbox;
                    WarningDomainLiteral.Text = domain;
                }
            }
            else
            {
                SenderWarningPanel.Visible = false;
            }
        }
    }
}