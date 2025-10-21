using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Accounts.Senders.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/senders/search";

        private Guid SenderID => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            DeleteButton.NavigateUrl = $"/ui/admin/accounts/senders/delete?id={SenderID}";
            CancelButton.NavigateUrl = SearchUrl;
        }

        private void Open()
        {
            var entity = TSenderSearch.Select(SenderID);
            if (entity == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{entity.SenderNickname} <span class='form-text'>{entity.SenderType}</span>");

            Details.SetInputValues(entity);

            OrganizationList.LoadData(entity.SenderIdentifier);

            MessageGrid.LoadData(entity.SenderIdentifier);

            MessagesSection.Visible = MessageGrid.RowCount > 0;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = TSenderSearch.Select(SenderID);

            Details.GetInputValues(entity);

            TSenderStore.Update(entity);

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }
    }
}