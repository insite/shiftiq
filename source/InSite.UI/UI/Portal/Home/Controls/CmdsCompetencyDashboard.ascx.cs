using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

namespace InSite.UI.Portal.Home.Controls
{
    public partial class CmdsCompetencyDashboard : BaseUserControl
    {
        private Guid LearnerIdentifier => Guid.TryParse(Request.QueryString["learner"], out Guid id) ? id : User.UserIdentifier;

        public CmdsComplianceSummary ComplianceSummary => CmdsComplianceSummary;
        public CmdsLearningSummary LearningSummary => CmdsLearningSummary;
        public CmdsOrientationSummary OrientationSummary => CmdsOrientationSummary;

        public event EventHandler DataLoaded;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowWhat.AutoPostBack = true;
            ShowWhat.ValueChanged += (x, y) => BindModelToControls(ShowWhat.Value);

            CmdsSecondaryEmployeeProfile.AutoPostBack = true;
            CmdsSecondaryEmployeeProfile.ValueChanged += (s, a) => LoadSecondaryEmployeeProfileSummary(CmdsSecondaryEmployeeProfile.ValueAsKey);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls("Primary");

            UserCompetencySummary.BindModelToControls(LearnerIdentifier);

            var hasData = TitlePanel.Visible
                || CmdsPrimaryProfilePanel.Visible
                || CmdsCompetencySummaryPanel.Visible
                || CmdsLearningSummaryPanel.Visible
                || UserCompetencySummary.Visible;

            if (!hasData)
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("There is no Competency data to display related to your learner profile."));

            DataLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void BindModelToControls(string showWhat)
        {
            bool isE03 = ServiceLocator.Partition.IsE03();

            TitlePanel.Visible = isE03;
            CmdsCompetencySummaryPanel.Visible = isE03;
            CmdsLearningSummaryPanel.Visible = isE03;

            CmdsLearningSummary.BindModelToControls(LearnerIdentifier, Organization.Identifier);
            CmdsOrientationSummary.BindModelToControls(LearnerIdentifier, Organization.Identifier);

            var key = LoadPrimaryProfile();

            LoadComplianceProfileSummary(key, showWhat);
        }

        private void LoadComplianceProfileSummary(UserProfileKey key, string showWhat)
        {
            CmdsPrimaryProfilePanel.Visible = false;

            if (showWhat == "Primary")
            {
                if (key == null)
                {
                    key = new UserProfileKey();
                    key.UserIdentifier = LearnerIdentifier;
                }
                else
                {
                    var profile = StandardSearch.Select(key.ProfileStandardIdentifier);
                    CmdsPrimaryProfileName.InnerHtml = $"{profile.Code}: {profile.ContentTitle}";
                    CmdsPrimaryProfilePanel.Visible = true;
                }

                CmdsPrimaryCompetencySummary.BindModelToControls(key.UserIdentifier, key.ProfileStandardIdentifier, key.DepartmentIdentifier);
                CmdsComplianceSummary.BindModelToControls(LearnerIdentifier, Organization.Identifier, key.ProfileStandardIdentifier);

            }
            else
            {
                CmdsPrimaryCompetencySummary.BindModelToControls(LearnerIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, CompetencySummaryType.EmployeeComplianceProfiles);
                CmdsComplianceSummary.BindModelToControls(LearnerIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);
            }
        }

        private void LoadSecondaryEmployeeProfileSummary(UserProfileKey key)
        {
            if (key == null) key = new UserProfileKey();

            CmdsSecondaryProfileSummary.Visible = true;
            CmdsSecondaryCompetencySummary.BindModelToControls(key.UserIdentifier, key.ProfileStandardIdentifier, key.DepartmentIdentifier);
        }

        private UserProfileKey LoadPrimaryProfile()
        {
            var info = UserProfileRepository.SelectPrimaryProfile(LearnerIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);

            if (info != null)
            {
                var key = new UserProfileKey
                {
                    DepartmentIdentifier = info.DepartmentIdentifier,
                    UserIdentifier = info.UserIdentifier,
                    ProfileStandardIdentifier = info.ProfileStandardIdentifier
                };
                return key;
            }

            return null;
        }
    }
}