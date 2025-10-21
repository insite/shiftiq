using System;
using System.Web.UI;

using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Messages.Forms
{
    public partial class Retitle : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid MessageIdentifier => Guid.TryParse(Request["message"], out var value) ? value : Guid.Empty;

        private const string SearchUrl = "/ui/admin/messages/messages/search";

        string DefaultParameters => $"message={MessageIdentifier}&tab=message";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => GetParentLinkParameters(parent, null).IfNullOrEmpty($"message={MessageIdentifier}");

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

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

            PageHelper.AutoBindHeader(Page, null, $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>");

            var content = ServiceLocator.ContentSearch.GetBlock(message.MessageIdentifier, labels: new[] { ContentLabel.Title });

            SubjectInput.Value = content.Title.Text;

            CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private void Save()
        {
            ServiceLocator.SendCommand(new RetitleMessage(MessageIdentifier, SubjectInput.Value));

            HttpResponseHelper.Redirect(GetParentUrl(DefaultParameters));
        }
    }
}