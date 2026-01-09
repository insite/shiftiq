using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Senders.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid SenderIdentifier => Guid.TryParse(Request["id"], out var asset) ? asset : Guid.Empty;

        private TSender Entity => _entity ?? (_entity = TSenderSearch.Select(SenderIdentifier));

        private const string FinderRelativePath = "/ui/admin/accounts/senders/search";

        #endregion

        #region Fields

        private TSender _entity;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = HttpResponseHelper.BuildUrl("/ui/admin/accounts/senders/edit", GetEditorParameters());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanDelete)
                HttpResponseHelper.Redirect(FinderRelativePath);

            if (Entity == null)
                HttpResponseHelper.Redirect(FinderRelativePath);

            PageHelper.AutoBindHeader(
                Page, 
                qualifier: $"{Entity.SenderNickname} <span class='form-text'>{Entity.SenderType}</span>");

            Nickname.Text = $"<a href='edit?id={SenderIdentifier}'>{Entity.SenderNickname}</a>";
            SenderType.Text = Entity.SenderType;
            FromName.Text = $"<a href='edit?id={SenderIdentifier}'>{Entity.SenderName}</a>";
            FromEmail.Text = $"<a href='mailto:{Entity.SenderEmail}'>{Entity.SenderEmail}</a>";
            SystemMailbox.Text = $"<a href='mailto:{Entity.SystemMailbox}'>{Entity.SystemMailbox}</a>";
            SenderStatus.Text = Entity.SenderEnabled ? "Enabled" : "Disabled";

            OrganizationsCount.Text = TSenderOrganizationSearch
                .Count(new TSenderOrganizationFilter { SenderIdentifier = SenderIdentifier })
                .ToString();
            MessagesCount.Text = ServiceLocator.MessageSearch
                .CountMessages(new MessageFilter { SenderIdentifier = SenderIdentifier })
                .ToString();
        }

        #endregion

        #region Event handlers

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            TSenderStore.Delete(Entity.SenderIdentifier);

            HttpResponseHelper.Redirect(FinderRelativePath);
        }

        #endregion

        #region Methods (helpers)

        private string GetEditorParameters()
        {
            return $"id={SenderIdentifier}";
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? GetEditorParameters()
                : null;
        }

        #endregion
    }
}