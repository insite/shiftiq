using System;
using System.Web.UI;

using InSite.Application.Contents.Read;
using InSite.Application.Journals.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks
{
    public partial class ChangeJournalComment : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid CommentIdentifier => Guid.TryParse(Request.QueryString["comment"], out var value) ? value : Guid.Empty;

        private Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

        private Guid JournalSetupIdentifier
        {
            get => (Guid)ViewState[nameof(JournalSetupIdentifier)];
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        private Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

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

            var command = new ChangeComment(
                JournalIdentifier,
                CommentIdentifier,
                entity.CommentText,
                DateTimeOffset.UtcNow,
                entity.CommentIsPrivate
            );

            ServiceLocator.SendCommand(command);

            var url = GetReturnUrlInternal();

            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            var comment = ServiceLocator.JournalSearch.GetJournalComment(CommentIdentifier);
            if (comment?.LogbookIdentifier == null || comment.AuthorUserIdentifier != User.UserIdentifier)
                RedirectToSearch();

            var journal = ServiceLocator.JournalSearch.GetJournal(comment.LogbookIdentifier.Value,
                x => x.User,
                x => x.JournalSetup,
                x => x.Experiences);

            if (journal == null || journal.JournalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                RedirectToSearch();

            JournalIdentifier = journal.JournalIdentifier;
            JournalSetupIdentifier = journal.JournalSetupIdentifier;
            UserIdentifier = journal.UserIdentifier;

            var title = $"{journal.JournalSetup.JournalSetupName} <span class='form-text'>{journal.User.UserFullName}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            var experienceIdentifier = comment.LogbookExperienceIdentifier;

            if (!Detail.SetInputValues(journal.JournalSetupIdentifier, User.UserIdentifier, comment))
            {
                RedirectToSearch();
                return;
            }

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);

        private string GetReturnUrlInternal() => GetReturnUrl()
            .IfNullOrEmpty($"/ui/admin/records/logbooks/outline-journal?journalSetup={JournalSetupIdentifier}&user={UserIdentifier}&panel=comments");

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/records/logbooks/searchjournal/search");
    }
}
