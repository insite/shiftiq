using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class ChangeEntry : PortalBasePage, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var id) ? id : Guid.Empty;

        private Guid JournalSetupIdentifier
        {
            get => (Guid)ViewState[nameof(JournalSetupIdentifier)];
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton1.Click += SaveButton_Click;
            SaveButton2.Click += SaveButton_Click;
            NextButton.Click += BackButton_Click;
            NextButton1.Click += NextButton1_Click;
            BackButton.Click += BackButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FieldsTab.IsSelected = true;
        }

        private void NextButton1_Click(object sender, EventArgs e)
        {
            CompetenciesTab.IsSelected = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Competencies.SetMaxHours(Fields.GetHours());

            Page.Validate(SaveButton1.ValidationGroup);

            if (!Page.IsValid)
                return;

            var commands = new List<Command>();

            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier);

            Fields.GetChanges(JournalSetupIdentifier, experience.JournalIdentifier, experience.ExperienceIdentifier, commands);
            Competencies.GetChanges(experience.JournalIdentifier, experience.ExperienceIdentifier, commands);

            ServiceLocator.SendCommands(commands);

            var url = $"/ui/portal/records/logbooks/outline?journalsetup={JournalSetupIdentifier}";
            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier, x => x.Journal.JournalSetup);

            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || experience.Journal.UserIdentifier != User.UserIdentifier
                || experience.ExperienceValidated.HasValue
                )
            {
                HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search");
                return;
            }

            var journalSetup = experience.Journal.JournalSetup;
            var content = ServiceLocator.ContentSearch.GetBlock(journalSetup.JournalSetupIdentifier);

            JournalSetupIdentifier = journalSetup.JournalSetupIdentifier;

            LoadDataDetails(experience, content);

            CancelButton1.NavigateUrl = $"/ui/portal/records/logbooks/outline?journalsetup={JournalSetupIdentifier}";
            CancelButton2.NavigateUrl = CancelButton1.NavigateUrl;

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text.Get(Identity.Language) ?? journalSetup.JournalSetupName);
        }

        private void LoadDataDetails(QExperience experience, ContentContainer content)
        {
            var instructions = content?[JournalSetupState.ContentLabels.Instructions].Text.Get(Identity.Language);

            InstructionTab.Visible = !string.IsNullOrEmpty(instructions);

            if (!string.IsNullOrEmpty(instructions))
                InstructionBody.Text = Markdown.ToHtml(instructions);

            var fieldCount = Fields.LoadData(JournalSetupIdentifier, experience);
            FieldsTab.Visible = fieldCount > 0;

            var competencyCount = Competencies.LoadData(JournalSetupIdentifier, experience.ExperienceIdentifier);
            CompetenciesTab.Visible = competencyCount > 0;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/outline-entry"))
                return $"experience={ExperienceIdentifier}";

            if (parent.Name.EndsWith("/outline"))
                return $"journalsetup={JournalSetupIdentifier}";

            return null;
        }
    }
}