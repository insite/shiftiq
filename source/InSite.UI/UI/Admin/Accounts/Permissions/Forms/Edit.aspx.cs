using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Permissions.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/permissions/search";

        private Guid PermissionID => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        private string ReturnUrl
        {
            get => ViewState[nameof(ReturnUrl)] as string;
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
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

            ReturnUrl = GetReturnUrl();

            Open();

            CancelButton.NavigateUrl = ReturnUrl ?? SearchUrl;
        }

        private void Open()
        {
            PageHelper.AutoBindHeader(Page);

            if (PermissionID.Equals(Guid.Empty))
                HttpResponseHelper.Redirect(SearchUrl);

            var permission = TGroupPermissionSearch.Select(PermissionID);

            var action = TActionSearch.Get(permission.ObjectIdentifier);
            if (action == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var group = ServiceLocator.GroupSearch.GetGroup(permission.GroupIdentifier);
            if (group == null)
                HttpResponseHelper.Redirect(SearchUrl);


            Detail.SetInputValues(action, group);

            var actions = TActionSearch.Search(x => x.PermissionParentActionIdentifier == permission.ObjectIdentifier).OrderBy(x => x.ActionUrl).ToList();
            if (actions.Count > 0)
            {
                ActionField.Visible = true;
                ActionRepeater.DataSource = actions;
                ActionRepeater.DataBind();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Detail.Save(out _);

            if (ReturnUrl.IsNotEmpty())
                HttpResponseHelper.Redirect(ReturnUrl, true);

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TGroupPermissionStore.Delete(PermissionID);
            HttpResponseHelper.Redirect(SearchUrl);
        }
    }
}