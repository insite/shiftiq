using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contents.Read;
using InSite.Application.Journals.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class CreateComment : PortalBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var id) ? id : Guid.Empty;
        private Guid? ExperienceIdentifier => Guid.TryParse(Request.QueryString["experience"], out var value) ? value : (Guid?)null;

        private Guid UserIdentifier => User.UserIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new QComment();
            Detail.GetInputValues(entity);

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

            var commentIdentifier = UniqueIdentifier.Create();
            var subjectIdentifier = ExperienceIdentifier ?? journalIdentifier;
            var subjectType = ExperienceIdentifier.HasValue ? "Experience" : "Journal";

            commands.Add(new AddComment(
                journalIdentifier,
                commentIdentifier,
                UserIdentifier,
                subjectIdentifier,
                subjectType,
                entity.CommentText,
                DateTimeOffset.UtcNow,
                false
            ));

            ServiceLocator.SendCommands(commands);

            var url = GetReturnUrlInternal();
            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            var journalSetupUser = ServiceLocator.JournalSearch.GetJournalSetupUser(
                JournalSetupIdentifier,
                UserIdentifier,
                JournalSetupUserRole.Learner
            );

            if (journalSetupUser == null)
            {
                RedirectToSearch();
                return;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier);

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text.Get(Identity.Language) ?? journalSetup.JournalSetupName);

            Detail.SetDefaultInputValues(JournalSetupIdentifier, UserIdentifier, ExperienceIdentifier);

            CancelButton.NavigateUrl = GetReturnUrlInternal();
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, $"journalsetup={JournalSetupIdentifier}&panel=comments");

        private string GetReturnUrlInternal() => GetReturnUrl()
            .IfNullOrEmpty($"/ui/portal/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=comments");

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search");
    }
}