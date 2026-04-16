using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.User.Achievements.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Learning
{
    public abstract class BasePlanPage : PortalBasePage, ICmdsUserControl
    {
        protected Guid EmployeeID => Guid.TryParse(Request["userID"], out var employeeID) ? employeeID : User.UserIdentifier;

        protected Guid? CredentialIdentifier => Guid.TryParse(Request["credential"], out var credential) ? credential : (Guid?)null;

        protected string AchievementType => Request["achievementType"];

        protected bool ShowAllAchievementsParam => Request["showAllAchievements"] == "1";

        protected abstract string PageUrl { get; }

        protected override void OnPreInit(EventArgs e)
        {
            MasterPageFile = ServiceLocator.Partition.IsE03()
                ? "~/UI/Layout/Admin/AdminHome.master"
                : "~/UI/Layout/Portal/Portal.master";

            base.OnPreInit(e);
        }

        protected abstract Toggle GetDisplayToggle();
        protected abstract Repeater GetProfilesInTrainingRepeater();
        protected abstract Control GetProfilesInTrainingSection();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GetProfilesInTrainingRepeater().ItemDataBound += ProfilesInTraining_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            GetDisplayToggle().Checked = ShowAllAchievementsParam;

            if (Master is PortalMaster portal)
                portal.SidebarVisible(false);

            if (Master is AdminHome admin)
                admin.HideHelpColumn();
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            var permissions = PermissionCache.Matrix.GetPermissions(Organization.Code);

            var roleNames = Identity.GetRoleNames();

            if (permissions.IsDenied("ui/home#training-plan", roleNames))
                HttpResponseHelper.SendHttp403();
        }

        private void ProfilesInTraining_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = (DataRowView)e.Item.DataItem;
            var profileStandardIdentifier = (Guid)row["ProfileStandardIdentifier"];
            var organization = (Guid)row["OrganizationIdentifier"];

            var complianceSummary = (ComplianceSummary)e.Item.FindControl("ComplianceSummary");

            complianceSummary.LoadData(EmployeeID, organization);
        }

        protected void LoadProfilesInTraining()
        {
            var employeeProfiles = UserProfileRepository.SelectSecondaryProfilesInTraining(EmployeeID, Organization.Identifier);

            GetProfilesInTrainingSection().Visible = employeeProfiles.Rows.Count > 0;

            if (employeeProfiles.Rows.Count == 0)
                return;

            var repeater = GetProfilesInTrainingRepeater();
            repeater.DataSource = employeeProfiles;
            repeater.DataBind();
        }

        protected string GetUrl(Guid? credentialId)
        {
            var parameters = string.Empty;

            if (credentialId.HasValue)
                parameters += $"&credential={credentialId.Value}";

            if (EmployeeID != User.UserIdentifier)
                parameters += $"&userID={EmployeeID}";

            if (AchievementType.IsNotEmpty())
                parameters += $"&achievementType={AchievementType}";

            if (GetDisplayToggle().Checked)
                parameters += "&showAllAchievements=1";

            if (parameters.Length > 0)
                parameters = "?" + parameters.Substring(1);

            return PageUrl + parameters;
        }
    }
}