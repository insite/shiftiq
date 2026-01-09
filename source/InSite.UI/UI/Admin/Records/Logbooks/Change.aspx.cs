using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventIdentifier.AutoPostBack = true;
            EventIdentifier.ValueChanged += EventIdentifier_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);
            PageHelper.AutoBindHeader(this, null, header);

            EventIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventIdentifier.Filter.EventType = "Class";
            EventIdentifier.Value = journalSetup.EventIdentifier;

            FrameworkStandardIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            FrameworkStandardIdentifier.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Framework };
            FrameworkStandardIdentifier.Value = journalSetup.FrameworkStandardIdentifier;

            var hasExperiences = ServiceLocator.JournalSearch.ExperienceExists(new QExperienceFilter { JournalSetupIdentifier = JournalSetupIdentifier });
            FrameworkStandardIdentifier.Enabled = !hasExperiences;
            ChangeFrameworkNotAllowed.Visible = hasExperiences;

            AchievementIdentifier.Value = journalSetup.AchievementIdentifier;

            IsValidationRequired.SelectedValue = journalSetup.IsValidationRequired.ToString().ToLower();

            bool isDownloadable = journalSetup.AllowLogbookDownload.HasValue && journalSetup.AllowLogbookDownload.Value;
            AllowLogbookDownload.Checked = isDownloadable;

            ValidatorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            ValidatorMessageIdentifier.Value = journalSetup.ValidatorMessageIdentifier;

            LearnerMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            LearnerMessageIdentifier.Value = journalSetup.LearnerMessageIdentifier;

            LearnerAddedMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            LearnerAddedMessageIdentifier.Value = journalSetup.LearnerAddedMessageIdentifier;

            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=setup";
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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var allowLogbookDownloadCommand = AllowLogbookDownload.Checked
                ? (Command)new AllowLogbookDownload(JournalSetupIdentifier)
                : (Command)new DisallowLogbookDownload(JournalSetupIdentifier);

            var commands = new List<Command>
            {
                new ChangeJournalSetupAchievement(JournalSetupIdentifier, AchievementIdentifier.Value),
                new ChangeJournalSetupEvent(JournalSetupIdentifier, EventIdentifier.Value),
                new ChangeJournalSetupIsValidationRequired(JournalSetupIdentifier, bool.Parse(IsValidationRequired.SelectedValue)),
                allowLogbookDownloadCommand,
                new ChangeJournalSetupMessages(
                        JournalSetupIdentifier,
                        ValidatorMessageIdentifier.Value,
                        LearnerMessageIdentifier.Value,
                        LearnerAddedMessageIdentifier.Value
                )
            };

            ChangeFramework(commands);

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=setup");
        }

        private void ChangeFramework(List<Command> commands)
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup.FrameworkStandardIdentifier == FrameworkStandardIdentifier.Value)
                return;

            var competencies = ServiceLocator.JournalSearch.GetCompetencyRequirements(JournalSetupIdentifier);
            foreach (var competency in competencies)
                commands.Add(new DeleteCompetencyRequirement(competency.JournalSetupIdentifier, competency.CompetencyStandardIdentifier));

            commands.Add(new ChangeJournalSetupFramework(JournalSetupIdentifier, FrameworkStandardIdentifier.Value));
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=setup"
                : null;
        }
    }
}
