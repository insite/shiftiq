using System;

using InSite.Application.Users.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class EditUserConnection : AdminBasePage, IHasParentLinkParameters
    {
        private const string EditUrl = "/ui/admin/contacts/people/edit";

        private const string SearchUrl = "/ui/admin/contacts/people/search";

        private Guid? FromUserIdentifier => Guid.TryParse(Request["from"], out var value) ? value : (Guid?)null;

        private Guid? ToUserIdentifier => Guid.TryParse(Request["to"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var connection = FromUserIdentifier.HasValue && ToUserIdentifier.HasValue
                ? UserConnectionSearch.Select(FromUserIdentifier.Value, ToUserIdentifier.Value, x => x.FromUser, x => x.ToUser)
                : null;

            if (connection == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: connection.FromUser.FullName);

            FromUserName.Text = connection.FromUser.FullName;
            ToUserName.Text = connection.ToUser.FullName;

            ConnectedOn.Value = connection.Connected;

            IsLeader.Checked = connection.IsLeader;
            IsManager.Checked = connection.IsManager;
            IsSupervisor.Checked = connection.IsSupervisor;
            IsValidator.Checked = connection.IsValidator;

            CancelButton.NavigateUrl = GetBackUrl();
        }

        private void Save()
        {
            var c = UserConnectionSearch.Select(FromUserIdentifier.Value, ToUserIdentifier.Value);

            if (ConnectedOn.Value.HasValue)
                c.Connected = ConnectedOn.Value.Value;

            c.IsLeader = IsLeader.Checked;
            c.IsManager = IsManager.Checked;
            c.IsSupervisor = IsSupervisor.Checked;
            c.IsValidator = IsValidator.Checked;

            ServiceLocator.SendCommand(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));

            HttpResponseHelper.Redirect(GetBackUrl());
        }

        private string GetBackUrl()
            => EditUrl + $"?contact={FromUserIdentifier}&panel=people";

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={FromUserIdentifier}&panel=people"
                : null;
        }
    }
}