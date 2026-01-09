using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks.Learners
{
    public partial class AddEntry : PortalBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var id) ? id : Guid.Empty;

        private Guid UserIdentifier => Guid.TryParse(Request["user"], out var id) ? id : User.UserIdentifier;

        private bool IsAdmin => UserIdentifier != User.UserIdentifier;

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

        public override void ApplyAccessControl()
        {
            if (IsAdmin
                    && !Identity.IsGranted(PermissionIdentifiers.Admin_Records)
                    && (
                        !Identity.IsGranted(PermissionIdentifiers.Portal_Logbooks)
                        || !ServiceLocator.JournalSearch.ExistsJournalSetupUser(JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator)
                    )
                )
            {
                CreateAccessDeniedException();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Competencies.SetMaxHours(Fields.GetHours());

            Page.Validate(SaveButton1.ValidationGroup);

            if (!Page.IsValid)
                return;

            var commands = new List<Command>();

            EnsureEnrolled(commands);

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
            commands.Add(new AddExperience(journalIdentifier, experienceIdentifier));

            Fields.GetChanges(JournalSetupIdentifier, journalIdentifier, experienceIdentifier, commands);
            Competencies.GetChanges(journalIdentifier, experienceIdentifier, commands);

            ServiceLocator.SendCommands(commands);

            var url = GetOutlineUrl();
            HttpResponseHelper.Redirect(url);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FieldsTab.IsSelected = true;
        }

        private void NextButton1_Click(object sender, EventArgs e)
        {
            Competencies.SetMaxHours(null);

            Page.Validate(SaveButton1.ValidationGroup);

            if (!Page.IsValid)
                return;

            CompetenciesTab.IsSelected = true;
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            if (!IsEnrolled())
            {
                RedirectToSearch();
                return;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier);

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text?.Get(Identity.Language) ?? journalSetup.JournalSetupName);

            LoadDataDetails(content);

            CancelButton1.NavigateUrl = GetOutlineUrl();
            CancelButton2.NavigateUrl = CancelButton1.NavigateUrl;
            CancelButton.NavigateUrl = CancelButton1.NavigateUrl;
        }

        private void LoadDataDetails(ContentContainer content)
        {
            var instructions = content?[JournalSetupState.ContentLabels.Instructions].Text.Get(Identity.Language);

            InstructionTab.Visible = !string.IsNullOrEmpty(instructions);

            if (!string.IsNullOrEmpty(instructions))
                InstructionBody.Text = Markdown.ToHtml(instructions);

            var fieldCount = Fields.LoadData(JournalSetupIdentifier, null);
            var competencyCount = Competencies.LoadData(JournalSetupIdentifier, null);

            FieldsTab.Visible = fieldCount > 0;
            CompetenciesTab.Visible = competencyCount > 0;

            if (fieldCount > 0)
            {
                SaveButton1.Visible = competencyCount == 0;
                NextButton1.Visible = competencyCount > 0;
                BackButton.Visible = true;
            }
            else
            {
                BackButton.Visible = false;

                StatusAlert.AddMessage(AlertType.Warning, $"You are not yet able to add entries to this logbook as configuration is incomplete. Please contact your administrator.");
            }
        }

        private bool IsEnrolled()
        {
            return ServiceLocator.JournalSearch.GetEnrollmentStatus(JournalSetupIdentifier, UserIdentifier) != LogbookEnrollmentStatus.NotEnrolled;
        }

        private void EnsureEnrolled(List<Command> commands)
        {
            var status = ServiceLocator.JournalSearch.GetEnrollmentStatus(JournalSetupIdentifier, UserIdentifier);
            if (status == LogbookEnrollmentStatus.NotEnrolled)
                RedirectToSearch();

            if (status == LogbookEnrollmentStatus.UserEnrolled)
                return;

            commands.Add(new AddJournalSetupUser(JournalSetupIdentifier, UserIdentifier, JournalSetupUserRole.Learner));
        }

        IWebRoute IOverrideWebRouteParent.GetParent() => GetParent();

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return GetParentLinkParameters(parent.Name);
        }

        private string GetParentLinkParameters(string parentName)
        {
            if (parentName.EndsWith("ui/portal/records/logbooks/outline"))
                return $"journalsetup={JournalSetupIdentifier}";

            if (parentName.EndsWith("ui/admin/records/logbooks/outline-journal"))
                return $"journalsetup={JournalSetupIdentifier}&user={UserIdentifier}";

            return null;
        }

        private void RedirectToSearch()
        {
            var searchUrl = "/ui/admin/records/logbooks/searchjournal/search";

            var parent = GetParent();
            while (parent != null)
            {
                if (parent.Name.EndsWith("/search"))
                {
                    searchUrl = "/" + parent.Name;
                    break;
                }

                parent = parent.GetParent();
            }

            HttpResponseHelper.Redirect(searchUrl);
        }

        private string GetOutlineUrl()
        {
            var parentUrl = GetParentUrl(null);

            var parameters = GetParentLinkParameters(parentUrl);
            if (parameters.IsNotEmpty())
                parentUrl += "?" + parameters;

            return parentUrl;
        }
    }
}