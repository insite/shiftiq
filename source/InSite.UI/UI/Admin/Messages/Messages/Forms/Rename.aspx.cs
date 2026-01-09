using System;
using System.Web.UI;

using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Messages.Messages.Forms
{
    public partial class Rename : AdminBasePage, IHasParentLinkParameters
    {
        private Guid MessageIdentifier => Guid.TryParse(Request["message"], out var value) ? value : Guid.Empty;

        private const string SearchUrl = "/ui/admin/messages/messages/search";

        private string OutlineUrl => $"/ui/admin/messages/outline?message={MessageIdentifier}&tab=message";

        public string GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/outline") ? $"message={MessageIdentifier}" : null;

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

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{message.MessageName} <span class='form-text'>{message.MessageType}</span>");

            NameInput.Value = message.MessageName;

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void Save()
        {
            ServiceLocator.SendCommand(new RenameMessage(MessageIdentifier, NameInput.Value));

            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}