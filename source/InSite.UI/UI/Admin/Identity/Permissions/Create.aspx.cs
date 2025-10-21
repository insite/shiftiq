using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Identity.Permissions
{
    public partial class Create : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ActionIdentifier => Guid.TryParse(Request["action"], out var result) ? result : (Guid?)null;
        private Guid? GroupIdentifier => Guid.TryParse(Request["group"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveClicked;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                PermissionGroupIdentifier.Value = GroupIdentifier;
                PermissionGroupIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            }
        }

        private void SetInputValues(TAction action)
        {
            var title = action.ActionUrl;

            if (action.ActionIcon != null)
                title = $"<i class='{action.ActionIcon}'></i> " + title;

            PageHelper.AutoBindHeader(this, null, title);

            if (GroupIdentifier.HasValue)
            {
                var permission = TGroupPermissionSearch.SelectFirst(x => x.ObjectIdentifier == ActionIdentifier && x.GroupIdentifier == GroupIdentifier);
                if (permission != null)
                {
                    AllowRead.Checked = permission.AllowRead;
                    AllowWrite.Checked = permission.AllowWrite;

                    AllowCreate.Checked = permission.AllowCreate;
                    AllowDelete.Checked = permission.AllowDelete;

                    AllowAdministrate.Checked = permission.AllowAdministrate;
                    AllowConfigure.Checked = permission.AllowConfigure;
                }
            }

            CancelButton.NavigateUrl = $"/ui/admin/platform/routes/edit?id={ActionIdentifier}&panel=permission";
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        private void Open()
        {
            var action = ActionIdentifier.HasValue ? TActionSearch.Get(ActionIdentifier.Value) : null;
            if (action == null)
                RedirectToSearch();

            SetInputValues(action);
        }

        private void Save()
        {
            var group = PermissionGroupIdentifier.Value.Value;
            var permission = TGroupPermissionSearch.SelectFirst(x => x.ObjectIdentifier == ActionIdentifier && x.GroupIdentifier == group);

            if (permission == null)
            {
                permission = new TGroupPermission
                {
                    GroupIdentifier = group,
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    PermissionIdentifier = UniqueIdentifier.Create(),

                    ObjectIdentifier = ActionIdentifier.Value,
                    ObjectType = "Action",
                    
                    AllowRead = AllowRead.Checked,
                    AllowWrite = AllowWrite.Checked,
                    AllowCreate = AllowCreate.Checked,
                    AllowDelete = AllowDelete.Checked,
                    AllowAdministrate = AllowAdministrate.Checked,
                    AllowConfigure = AllowConfigure.Checked,

                    PermissionGranted = DateTimeOffset.Now,
                    PermissionGrantedBy = User.Identifier
                };

                TGroupPermissionStore.Insert(permission);
            }
            else
            {
                permission.GroupIdentifier = group;
                permission.AllowRead = AllowRead.Checked;
                permission.AllowWrite = AllowWrite.Checked;
                permission.AllowCreate = AllowCreate.Checked;
                permission.AllowDelete = AllowDelete.Checked;
                permission.AllowAdministrate = AllowAdministrate.Checked;
                permission.AllowConfigure = AllowConfigure.Checked;
                TGroupPermissionStore.Update(permission);
            }

            if (GroupIdentifier.HasValue && PermissionGroupIdentifier.Value != GroupIdentifier)
                TGroupPermissionStore.Delete(GroupIdentifier.Value, ActionIdentifier.Value);

            RedirectToActionReader();
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/platform/routes/search", true);

        private void RedirectToActionReader()
        {
            var query = $"id={ActionIdentifier}&panel=permission";

            HttpResponseHelper.Redirect("/ui/admin/platform/routes/edit?" + query, true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"id={ActionIdentifier}"
                : null;
        }
    }
}
