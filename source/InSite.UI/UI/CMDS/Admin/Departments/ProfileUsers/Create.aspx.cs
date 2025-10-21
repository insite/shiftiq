using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

namespace InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/cmds/admin/departments/profile-users/edit";
        private const string SearchUrl = "/ui/cmds/admin/departments/profile-users/search";

        private DepartmentProfileUser _entity;

        public override void ApplyAccessControlForCmds() { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this, null, "Acquire Profile");

            Detail.SetDefaultInputValues(Organization.Identifier);

            CancelButton.NavigateUrl = SearchUrl;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();

            var editUrl = $"{EditUrl}?department={_entity.DepartmentIdentifier}&profile={_entity.ProfileStandardIdentifier}&user={_entity.UserIdentifier}&status=saved";

            HttpResponseHelper.Redirect(editUrl);
        }

        private void Save()
        {
            _entity = new DepartmentProfileUser();

            Detail.GetInputValues(_entity);

            DepartmentProfileUserStore.Insert(_entity);

            if (_entity.IsPrimary)
                UserProfileRepository.ChangePrimaryUserProfile(_entity.UserIdentifier, _entity.ProfileStandardIdentifier, _entity.DepartmentIdentifier);
        }
    }
}
