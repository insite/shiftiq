using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Contacts.Groups
{
    public partial class Outline : AdminBasePage
    {
        private Guid GroupID => Guid.TryParse(Request["contact"], out var value) ? value : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var group = ServiceLocator.GroupSearch.GetGroup(GroupID);
            if (group == null || group.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/admin/contacts/groups/search", true);

            PageHelper.AutoBindHeader(Page, null, group.GroupName);

            GroupSection.SetInputValues(group);
            WorkflowSection.SetInputValues(group);
            OccupationSection.LoadProfiles(group.GroupIdentifier);
            AddressSection.SetInputValues(group.GroupIdentifier);
            RoleGrid.LoadData(group);
            PermissionGrid.LoadData(group.GroupIdentifier, "contact");

            var isRole = group.GroupType == GroupTypes.Role;

            PermissionSection.Visible = isRole && CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Settings);
            AddressSection.Visible = !isRole;
        }
    }
}