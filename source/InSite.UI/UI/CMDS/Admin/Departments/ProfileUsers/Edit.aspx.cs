using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string CreateUrl = "/ui/cmds/admin/departments/profile-users/create";
        private const string SearchUrl = "/ui/cmds/admin/departments/profile-users/search";

        public override void ApplyAccessControlForCmds() { }

        private Guid DepartmentIdentifier => Guid.TryParse(Request["department"], out var key) ? key : Guid.Empty;

        private Guid ProfileStandardIdentifier => Guid.TryParse(Request["profile"], out var key) ? key : Guid.Empty;

        private Guid UserIdentifier => Guid.TryParse(Request["user"], out var key) ? key : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var entity = DepartmentProfileUserSearch.SelectFirst(x => x.DepartmentIdentifier == DepartmentIdentifier && x.ProfileStandardIdentifier == ProfileStandardIdentifier && x.UserIdentifier == UserIdentifier, x => x.Profile, x => x.Department);
            if (entity != null)
                DepartmentProfileUserStore.Delete(new[] { entity }, User.UserIdentifier, Organization.OrganizationIdentifier);

            HttpResponseHelper.Redirect(SearchUrl);
        }

        private void Open()
        {
            var entity = DepartmentProfileUserSearch.SelectFirst(x => x.DepartmentIdentifier == DepartmentIdentifier && x.ProfileStandardIdentifier == ProfileStandardIdentifier && x.UserIdentifier == UserIdentifier, x => x.Profile, x => x.Department);
            if (entity == null || !Detail.SetInputValues(entity))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Create", CreateUrl), entity.Profile.ContentTitle);
        }

        private bool Save()
        {
            var entity = DepartmentProfileUserSearch.SelectFirst(x => x.DepartmentIdentifier == DepartmentIdentifier && x.ProfileStandardIdentifier == ProfileStandardIdentifier && x.UserIdentifier == UserIdentifier, x => x.Profile, x => x.Department);
            if (entity == null)
                return false;

            var oldIsPrimary = entity.IsPrimary;

            Detail.GetInputValues(entity);

            DepartmentProfileUserStore.Update(entity);

            if (entity.IsPrimary && !oldIsPrimary)
                UserProfileRepository.ChangePrimaryUserProfile(UserIdentifier, ProfileStandardIdentifier, DepartmentIdentifier);

            return true;
        }
    }
}
