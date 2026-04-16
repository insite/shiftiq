using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Learning.Controls;

using Shift.Constant;

namespace InSite.UI.Portal.Learning
{
    public partial class Plan : BasePlanPage
    {
        protected override string PageUrl => "/ui/portal/learning/plan";

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SignOff.CanBeSigned = Access.Write;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DisplayToggle.AutoPostBack = true;
            DisplayToggle.CheckedChanged += (s, a) => HttpResponseHelper.Redirect(GetUrl(null));

            SignOff.SignedOff += (s, a) => HttpResponseHelper.Redirect(GetUrl(null));
        }

        protected override Toggle GetDisplayToggle() => DisplayToggle;

        protected override Repeater GetProfilesInTrainingRepeater() => ProfilesInTraining;

        protected override Control GetProfilesInTrainingSection() => ProfilesInTrainingSection;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var person = UserSearch.Select(EmployeeID);

            PageHelper.AutoBindHeader(this, null, $"Training Plan for {person.FullName}");

            LoadData();
        }

        private void LoadData()
        {
            var credentialId = CredentialIdentifier;

            SignOff.LoadData(EmployeeID, credentialId, false);

            var credentials = VCmdsCredentialSearch.SelectForTrainingPlan(
                EmployeeID,
                Organization.Identifier,
                AchievementType);
            var dataSource = PlanAchievementTypeRepeater.GetDataSource(credentials, !DisplayToggle.Checked);

            AchievementTypes.SelectedCredentialId = credentialId;
            AchievementTypes.LoadData(dataSource.Items, GetUrl);

            var selectedCredential = credentials.FirstOrDefault(x => x.CredentialIdentifier == credentialId);
            if (selectedCredential != null)
                SignOff.LoadAchievementInfo(selectedCredential, false);

            var hasCredentials = credentials.Count > 0;

            if (!hasCredentials)
                PlanAlert.AddMessage(AlertType.Information, "Your training plan has not yet been set up by your administrator.");

            else if (credentials.Count == dataSource.ValidItemsCount)
                PlanAlert.AddMessage(AlertType.Success, "All the items in your training plan are complete!");

            DisplayTogglePanel.Visible = hasCredentials;
            AchievementPanel.Visible = hasCredentials;

            LoadProfilesInTraining();
        }
    }
}
