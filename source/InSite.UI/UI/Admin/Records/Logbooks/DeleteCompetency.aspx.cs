using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks
{
    public partial class DeleteCompetency : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;
        private Guid CompetencyStandardIdentifier => Guid.TryParse(Request["competency"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var competency = ServiceLocator.JournalSearch.GetCompetencyRequirement(JournalSetupIdentifier, CompetencyStandardIdentifier,
                    x => x.JournalSetup, 
                    x => x.JournalSetup.Event,
                    x => x.JournalSetup.Achievement,
                    x => x.JournalSetup.Framework);

                if (competency == null || competency.JournalSetup.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, LogbookHeaderHelper.GetLogbookHeader(competency.JournalSetup, User.TimeZone));

                LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}\">{competency.JournalSetup.JournalSetupName}</a>";
                var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier, MultilingualString.DefaultLanguage);
                var title = content?.Title?.Text.Default;
                LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";

                LoadData(competency);

                CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=competencies";
            }
        }

        private void LoadData(QCompetencyRequirement competency)
        {
            var standard = StandardSearch.SelectFirst(x => x.StandardIdentifier == competency.CompetencyStandardIdentifier);

            Name.Text = $"<a href=\"/ui/admin/standards/edit?id={competency.CompetencyStandardIdentifier}\">{CompetencyHelper.GetStandardName(standard)}</a>";
            Hours.Text = competency.CompetencyHours.HasValue ? $"{competency.CompetencyHours:n2}" : "None";
            JournalItems.Text = competency.JournalItems.HasValue ? $"{competency.JournalItems:n0}" : "None";
            SkillRating.Text = competency.SkillRating.HasValue ? competency.SkillRating.ToString() : "None";

            var journalCount = ServiceLocator.JournalSearch.CountJournals(new QJournalFilter
            {
                JournalSetupIdentifier = JournalSetupIdentifier,
                CompetencyStandardIdentifier = CompetencyStandardIdentifier
            });

            DeleteJournalItems.Text = $"{journalCount:0}";

            if (journalCount > 0)
                DeleteConfirmationCheckbox.Text = "Delete Competency and all its values in Log Entries";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var experiences = ServiceLocator.JournalSearch.GetExperiences(new QExperienceFilter
            {
                JournalSetupIdentifier = JournalSetupIdentifier,
                CompetencyStandardIdentifier = CompetencyStandardIdentifier
            });

            var commands = new List<Command>();
            foreach (var experience in experiences)
                commands.Add(new DeleteExperienceCompetency(experience.JournalIdentifier, experience.ExperienceIdentifier, CompetencyStandardIdentifier));

            commands.Add(new DeleteCompetencyRequirement(JournalSetupIdentifier, CompetencyStandardIdentifier));

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=competencies");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=competencies"
                : null;
        }
    }
}