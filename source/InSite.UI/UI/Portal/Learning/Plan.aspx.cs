using System;
using System.Collections.Generic;
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

            ProgramFilter.AutoPostBack = true;
            ProgramFilter.SelectedIndexChanged += ProgramFilter_SelectedIndexChanged;
        }

        // Switching the program filter resets the plan to that program (or to all training),
        // dropping any achievement-type or credential context from the previous program.
        private void ProgramFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var parameters = string.Empty;

            if (Guid.TryParse(ProgramFilter.SelectedValue, out var programId))
                parameters += $"&program={programId}";

            if (EmployeeID != User.UserIdentifier)
                parameters += $"&userID={EmployeeID}";

            if (parameters.Length > 0)
                parameters = "?" + parameters.Substring(1);

            HttpResponseHelper.Redirect(PageUrl + parameters);
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

            PageHelper.AutoBindHeader(this, null, $"Training Plan for {(person?.FullName ?? "Unknown User")}");

            LoadData();
        }

        private void LoadData()
        {
            var credentialId = CredentialIdentifier;

            SignOff.LoadData(EmployeeID, credentialId, false);

            LoadProgramFilter();

            Guid[] achievementIds = null;
            if (ProgramIdentifier.HasValue)
                achievementIds = TaskSearch.SelectByProgram(ProgramIdentifier.Value)
                    .Select(x => x.AchievementIdentifier)
                    .Distinct()
                    .ToArray();

            // A program that defines no achievements would otherwise drop the filter, because an empty id
            // list means "no filter" to the search, and the whole plan would render under that program's
            // name. Show an empty plan with an explanation instead.
            var programHasNoAchievements = achievementIds != null && achievementIds.Length == 0;

            var credentials = programHasNoAchievements
                ? new List<VCmdsCredentialAndExperience>()
                : VCmdsCredentialSearch.SelectForTrainingPlan(
                    EmployeeID,
                    Organization.Identifier,
                    AchievementType,
                    achievementIds);

            var dataSource = PlanAchievementTypeRepeater.GetDataSource(credentials, !DisplayToggle.Checked);

            AchievementTypes.SelectedCredentialId = credentialId;
            AchievementTypes.LoadData(dataSource.Items, GetUrl);

            VCmdsCredentialAndExperience selectedCredential = null;

            if (credentialId.HasValue)
            {
                selectedCredential = credentials.FirstOrDefault(x => x.CredentialIdentifier == credentialId);

                if (selectedCredential == null)
                    selectedCredential = VCmdsCredentialSearch.SelectForTrainingPlan(credentialId.Value, Organization.Identifier);

                if (selectedCredential != null && selectedCredential.UserIdentifier != EmployeeID)
                    selectedCredential = null;
            }

            SignOff.LoadData(selectedCredential, false);

            var hasCredentials = credentials.Count > 0;

            if (!hasCredentials && ProgramIdentifier.HasValue)
                PlanAlert.AddMessage(AlertType.Information, programHasNoAchievements
                    ? "This program does not define any achievements yet, so there is nothing to show for it. Choose \"All training\" to see your whole plan."
                    : "None of this program's achievements are in your training plan yet. Choose \"All training\" to see your whole plan.");

            else if (!hasCredentials)
                PlanAlert.AddMessage(AlertType.Information, "Your training plan has not yet been set up by your administrator.");

            else if (credentials.Count == dataSource.ValidItemsCount)
                PlanAlert.AddMessage(AlertType.Success, "All the items in your training plan are complete!");

            DisplayTogglePanel.Visible = hasCredentials;
            AchievementPanel.Visible = hasCredentials;

            LoadProfilesInTraining();
        }

        // The program filter lists the programs the learner holds an enrollment in and defaults to
        // "All training". It is hidden entirely when the learner is enrolled in no program. Enrollments are
        // read across the signed-in organization and the partition - the same set the catalogue card routing
        // and the home page's My Programs tile use - so an enrollment an administrator recorded under the
        // partition still shows here.
        private void LoadProgramFilter()
        {
            var enrollments = ProgramSearch1.GetUserProgramEnrollments(
                EmployeeID,
                new[] { Organization.Identifier, ServiceLocator.Partition.Identifier }.Distinct().ToArray(),
                x => x.Program);

            ProgramFilterPanel.Visible = enrollments.Count > 0;
            if (enrollments.Count == 0)
                return;

            ProgramFilter.Items.Clear();
            ProgramFilter.Items.Add(new ListItem("All training", string.Empty));

            foreach (var enrollment in enrollments
                .Where(x => x.Program != null)
                .OrderBy(x => x.Program.ProgramName))
            {
                ProgramFilter.Items.Add(new ListItem(enrollment.Program.ProgramName, enrollment.ProgramIdentifier.ToString()));
            }

            if (ProgramIdentifier.HasValue)
            {
                var selected = ProgramFilter.Items.FindByValue(ProgramIdentifier.Value.ToString());
                if (selected != null)
                    selected.Selected = true;
            }
        }
    }
}
