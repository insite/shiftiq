using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Common.Web;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class AddExperience : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var id) ? id : Guid.Empty;

        private Guid UserIdentifier => Guid.TryParse(Request["user"], out var id) ? id : User.UserIdentifier;

        private bool IsAdmin => UserIdentifier != User.UserIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NextButton1.Click += BackButton_Click;

            NextButton2.Click += NextButton_Click;
            SaveButton2.Click += SaveButton_Click;

            SaveButton3.Click += SaveButton_Click;
            BackButton3.Click += BackButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Competencies.SetMaxHours(Fields.GetHours());

            if (!Page.IsValid)
                return;

            var commands = new List<Command>();

            Guid journalIdentifier;

            var journal = ServiceLocator.JournalSearch.GetJournal(JournalSetupIdentifier, UserIdentifier);
            if (journal == null)
            {
                journalIdentifier = UniqueIdentifier.Create();
                commands.Add(new CreateJournal(journalIdentifier, JournalSetupIdentifier, UserIdentifier));
            }
            else
                journalIdentifier = journal.JournalIdentifier;

            var experienceIdentifier = UniqueIdentifier.Create();
            commands.Add(new InSite.Application.Journals.Write.AddExperience(journalIdentifier, experienceIdentifier));

            Fields.GetChanges(JournalSetupIdentifier, journalIdentifier, experienceIdentifier, commands);
            Competencies.GetChanges(journalIdentifier, experienceIdentifier, commands);

            ServiceLocator.SendCommands(commands);

            var url = GetOutlineUrl();
            HttpResponseHelper.Redirect(url);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            Competencies.SetMaxHours(null);

            if (!Page.IsValid)
                return;

            CompetenciesTab.IsSelected = true;
        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            FieldsTab.IsSelected = true;
        }

        private void LoadData()
        {
            if (IsAdmin
                    && !Identity.IsGranted(PermissionIdentifiers.Admin_Records)
                    && (!Identity.IsGranted(PermissionIdentifiers.Portal_Logbooks)
                        || !ServiceLocator.JournalSearch.ExistsJournalSetupUser(JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator)
                    )
                )
            {
                RedirectToSearch();
                return;
            }

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            if (!ServiceLocator.JournalSearch.ExistsJournalSetupUser(JournalSetupIdentifier, UserIdentifier, JournalSetupUserRole.Learner))
            {
                RedirectToSearch();
                return;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier);

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text?.Get(Identity.Language) ?? journalSetup.JournalSetupName);

            LoadDataDetails(content);

            var outlineUrl = GetOutlineUrl();

            CancelButton1.NavigateUrl = outlineUrl;
            CancelButton2.NavigateUrl = outlineUrl;
        }

        private void LoadDataDetails(ContentContainer content)
        {
            var instructions = content?[JournalSetupState.ContentLabels.Instructions].Text.Get(Identity.Language);

            InstructionTab.Visible = !string.IsNullOrEmpty(instructions);

            if (!string.IsNullOrEmpty(instructions))
                InstructionBody.Text = Markdown.ToHtml(instructions);

            var fieldCount = Fields.LoadData(JournalSetupIdentifier, null);
            var competencyCount = Competencies.LoadData(JournalSetupIdentifier, null);

            var hasFields = fieldCount > 0;
            var hasCompetencies = competencyCount > 0;

            FieldsTab.Visible = hasFields;
            CompetenciesTab.Visible = hasCompetencies;

            SaveButton2.Visible = hasFields && !hasCompetencies;
            NextButton2.Visible = hasFields && hasCompetencies;
            BackButton3.Visible = hasFields;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("ui/admin/records/logbooks/validators/outline"))
                return $"journalsetup={JournalSetupIdentifier}";

            if (parent.Name.EndsWith("ui/admin/records/logbooks/validators/outline-journal"))
                return $"journalsetup={JournalSetupIdentifier}&user={UserIdentifier}";

            return null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() => GetParent();

        private void RedirectToSearch()
        {
            HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");
        }

        private string GetOutlineUrl()
        {
            return GetParentUrl($"journalsetup={JournalSetupIdentifier}&user={UserIdentifier}");
        }
    }
}