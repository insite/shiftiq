using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Cmds.User.Achievements.Controls;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Learning
{
    public partial class Plan : PortalBasePage, ICmdsUserControl
    {
        protected string GetAchievementTypeDisplay(string type)
        {
            var organization = Organization.Code;
            return Shift.Common.AchievementTypes.Pluralize(type, organization);
        }

        protected class PolicyItem
        {
            public IList<VCmdsCredentialAndExperience> Credentials { get; set; }
            public string SubType { get; set; }
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();
            SignOff.CanBeSigned = Access.Write;
        }

        private Guid EmployeeID => Guid.TryParse(Request["userID"], out var employeeID)
            ? employeeID
            : User.UserIdentifier;

        private Guid? CredentialIdentifier => Guid.TryParse(Request["credential"], out var credential) ? credential : (Guid?)null;

        private string AchievementType => Request["achievementType"];

        private bool ShowAllAchievementsParam => Request["showAllAchievements"] == "1";

        protected override void OnPreInit(EventArgs e)
        {
            MasterPageFile = ServiceLocator.Partition.IsE03()
                ? "~/UI/Layout/Admin/AdminHome.master"
                : "~/UI/Layout/Portal/Portal.master";

            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementTypes.ItemDataBound += AchievementTypes_ItemDataBound;
            AchievementTypes.ItemCreated += AchievementTypes_ItemCreated;

            ProfilesInTraining.ItemDataBound += ProfilesInTraining_ItemDataBound;

            DisplayToggle.AutoPostBack = true;
            DisplayToggle.CheckedChanged += ShowAllAchievements_CheckedChanged;

            SignOff.SignedOff += SignOff_SignedOff;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            DisplayToggle.Checked = ShowAllAchievementsParam;

            InitTitle();

            if (Master is PortalMaster portal)
                portal.SidebarVisible(false);

            if (Master is AdminHome admin)
                admin.HideHelpColumn();

            LoadData();
        }

        private static void AchievementTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var policyItem = (PolicyItem)e.Item.DataItem;

            var achievements = (Repeater)e.Item.FindControl("Achievements");
            achievements.DataSource = policyItem.Credentials;
            achievements.DataBind();
        }

        private void AchievementTypes_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var achievements = (Repeater)e.Item.FindControl("Achievements");

            achievements.ItemDataBound += Achievements_ItemDataBound;
        }

        private void Achievements_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var row = (VCmdsCredentialAndExperience)e.Item.DataItem;

            var tableRow = (HtmlTableRow)e.Item.FindControl("Row");
            var achievementLink = (HyperLink)e.Item.FindControl("AchievementLink");
            var warningPanel = e.Item.FindControl("WarningPanel");

            UI.CMDS.Portal.Achievements.Credentials.SearchResults.BindDataItem(e.Item, row, false);

            var planCredentialUrl = $"/ui/portal/learning/plan?credential={row.CredentialIdentifier}";

            if (EmployeeID != User.UserIdentifier)
                planCredentialUrl += $"&userID={EmployeeID}";

            if (!string.IsNullOrEmpty(AchievementType))
                planCredentialUrl += $"&achievementType={AchievementType}";

            if (DisplayToggle.Checked)
                planCredentialUrl += "&showAllAchievements=1";

            achievementLink.NavigateUrl = planCredentialUrl;

            if (CredentialIdentifier == row.CredentialIdentifier)
            {
                tableRow.Attributes["class"] = "selected";

                SignOff.LoadAchievementInfo(row);
            }

            warningPanel.Visible = row.CredentialStatus == "Valid" && row.CredentialGranted == null;
        }

        private void ProfilesInTraining_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var profileStandardIdentifier = (Guid)row["ProfileStandardIdentifier"];
            var organization = (Guid)row["OrganizationIdentifier"];

            var complianceSummary = (ComplianceSummary)e.Item.FindControl("ComplianceSummary");

            complianceSummary.LoadData(EmployeeID, organization);
        }

        private void ShowAllAchievements_CheckedChanged(object sender, EventArgs e)
        {
            var path = Request.Url.AbsolutePath;

            var parameters = new List<string>();

            if (DisplayToggle.Checked)
                parameters.Add("showAllAchievements=1");

            if (EmployeeID != User.UserIdentifier)
                parameters.Add($"userID={EmployeeID}");

            if (parameters.Any())
                path += "?" + string.Join("&", parameters);

            HttpResponseHelper.Redirect(path);
        }

        private void SignOff_SignedOff(object sender, EventArgs e)
        {
            var url = $"/ui/portal/learning/plan";

            var parameters = new List<string>();

            if (EmployeeID != User.UserIdentifier)
                parameters.Add($"userID={EmployeeID}");

            if (!string.IsNullOrEmpty(AchievementType))
                parameters.Add($"achievementType={AchievementType}");

            if (DisplayToggle.Checked)
                parameters.Add("showAllAchievements=1");

            if (parameters.Count > 0)
                url += "?" + string.Join("&", parameters);

            HttpResponseHelper.Redirect(url);
        }

        private void InitTitle()
        {
            var person = UserSearch.Select(EmployeeID);

            PageHelper.AutoBindHeader(this, null, $"Training Plan for {person.FullName}");
        }

        private void LoadData()
        {
            SignOff.LoadData(EmployeeID, CredentialIdentifier);

            var credentials = VCmdsCredentialSearch.SelectForTrainingPlan(
                EmployeeID,
                CurrentIdentityFactory.ActiveOrganizationIdentifier,
                AchievementType);

            var validCredentials = credentials.Where(x => x.CredentialStatus == "Valid").ToList();

            var invalidCredentials = credentials.Where(x => x.CredentialStatus != "Valid").ToList();

            var hasCredentials = credentials.Count > 0;

            if (!hasCredentials)
            {
                PlanAlert.AddMessage(AlertType.Information, "Your training plan has not yet been set up by your administrator.");
            }

            if (hasCredentials && credentials.Count == validCredentials.Count)
            {
                PlanAlert.AddMessage(AlertType.Success, "All the items in your training plan are complete!");
            }

            DisplayTogglePanel.Visible = hasCredentials;

            AchievementPanel.Visible = hasCredentials;

            if (hasCredentials)
            {
                if (DisplayToggle.Checked)
                {
                    AchievementTypes.DataSource = CreateList(credentials);
                    AchievementTypes.DataBind();
                }
                else
                {
                    AchievementTypes.DataSource = CreateList(invalidCredentials);
                    AchievementTypes.DataBind();
                }
            }

            var employeeProfiles = UserProfileRepository.SelectSecondaryProfilesInTraining(EmployeeID, CurrentIdentityFactory.ActiveOrganizationIdentifier);

            ProfilesInTrainingSection.Visible = employeeProfiles.Rows.Count > 0;

            if (employeeProfiles.Rows.Count > 0)
            {
                ProfilesInTraining.DataSource = employeeProfiles;
                ProfilesInTraining.DataBind();
            }
        }

        private static IList<PolicyItem> CreateList(List<VCmdsCredentialAndExperience> table)
        {
            var achievements = new List<PolicyItem>();

            foreach (var row in table)
            {
                var achievementType = row.AchievementLabel ?? "N/A";
                var item = FindPolicyItem(achievements, achievementType);
                item.Credentials.Add(row);
            }

            return achievements;
        }

        private static PolicyItem FindPolicyItem(List<PolicyItem> achievementTypes, string type)
        {
            foreach (var item in achievementTypes)
                if (item.SubType == type)
                    return item;

            var achievementType = new PolicyItem();
            achievementType.Credentials = new List<VCmdsCredentialAndExperience>();
            achievementType.SubType = type;

            achievementTypes.Add(achievementType);

            return achievementType;
        }
    }
}
