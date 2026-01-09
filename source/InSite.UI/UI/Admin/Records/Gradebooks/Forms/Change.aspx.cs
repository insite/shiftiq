using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IncludeValidator.ServerValidate += IncludeValidator_ServerValidate;

            EventIdentifier.AutoPostBack = true;
            EventIdentifier.ValueChanged += EventIdentifier_ValueChanged;

            Standards.AutoPostBack = true;
            Standards.CheckedChanged += Standards_CheckedChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier);

                if (gradebook == null || gradebook.OrganizationIdentifier != CurrentSessionState.Identity.Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/search");
                    return;
                }

                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
                var gradebookType = gradebook.GradebookType.ToEnum<GradebookType>();

                PageHelper.AutoBindHeader(this, null, gradebook.GradebookTitle);

                EventIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                EventIdentifier.Filter.EventType = "Class";
                EventIdentifier.Value = gradebook.EventIdentifier;
                EventIdentifier.Enabled = !data.IsLocked;

                StandardIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                StandardIdentifier.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Framework };
                StandardIdentifier.Value = gradebook.FrameworkIdentifier;
                StandardIdentifier.Enabled = !data.IsLocked && data.RootItems.Count == 0;

                AchievementIdentifier.Value = gradebook.AchievementIdentifier;
                AchievementIdentifier.Enabled = !data.IsLocked && (data.Enrollments.IsEmpty() || data.Achievement == null);

                Standards.Checked = gradebookType == GradebookType.Standards || gradebookType == GradebookType.ScoresAndStandards;
                Standards.Enabled = !data.IsLocked && data.RootItems.Count == 0;

                StandardField.Visible = gradebookType == GradebookType.Standards || gradebookType == GradebookType.ScoresAndStandards;

                Scores.Checked = gradebookType == GradebookType.Scores || gradebookType == GradebookType.ScoresAndStandards;
                Scores.Enabled = false;

                AchievementWarning.Visible = data.Enrollments.IsNotEmpty() && data.Achievement.HasValue;

                PeriodIdentifier.Value = gradebook.PeriodIdentifier;
                PeriodIdentifier.Enabled = !data.IsLocked;

                SaveButton.Visible = !data.IsLocked;

                CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=gradebook";
            }
        }

        private void EventIdentifier_ValueChanged(object sender, EventArgs e)
        {
            if (EventIdentifier.HasValue)
            {
                var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value.Value);
                var achievement = @event.AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(@event.AchievementIdentifier.Value) : null;

                if (achievement != null)
                    AchievementIdentifier.Value = achievement.AchievementIdentifier;
            }
        }

        private void IncludeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Scores.Checked || Standards.Checked;
        }

        private void Standards_CheckedChanged(object sender, EventArgs e)
        {
            StandardField.Visible = Standards.Checked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier);

            var commands = new List<Command>();

            if (gradebook.AchievementIdentifier != AchievementIdentifier.Value)
                commands.Add(new ChangeGradebookAchievement(GradebookIdentifier, AchievementIdentifier.Value));

            if (gradebook.EventIdentifier != EventIdentifier.Value)
            {
                if (EventIdentifier.Value.HasValue)
                    commands.Add(new AddGradebookEvent(GradebookIdentifier, EventIdentifier.Value.Value, true));
                else
                    commands.Add(new RemoveGradebookEvent(GradebookIdentifier, gradebook.EventIdentifier.Value));
            }

            GradebookType gradebookType;
            if (Scores.Checked && Standards.Checked)
                gradebookType = GradebookType.ScoresAndStandards;
            else if (Scores.Checked)
                gradebookType = GradebookType.Scores;
            else
                gradebookType = GradebookType.Standards;

            if (gradebook.GradebookType.ToEnum<GradebookType>() != gradebookType || gradebook.FrameworkIdentifier != StandardIdentifier.Value)
                commands.Add(new ChangeGradebookType(GradebookIdentifier, gradebookType, StandardIdentifier.Value));

            commands.Add(new ChangeGradebookPeriod(GradebookIdentifier, PeriodIdentifier.Value));

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            Course2Store.ClearCache(Organization.Identifier);

            HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=gradebook");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=gradebook"
                : null;
        }
    }
}
