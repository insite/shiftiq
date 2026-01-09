using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

using ChangeCommentCommand = InSite.Application.Journals.Write.ChangeComment;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class ChangeComment : PortalBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid CommentIdentifier => Guid.TryParse(Request["comment"], out var id) ? id : Guid.Empty;

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

            var entity = ServiceLocator.JournalSearch.GetJournalComment(CommentIdentifier);
            Detail.GetInputValues(entity);

            var command = new ChangeCommentCommand(JournalIdentifier, CommentIdentifier, entity.CommentText, DateTimeOffset.UtcNow, false);

            ServiceLocator.SendCommand(command);

            // var changed = entity.CommentPosted.UtcDateTime.ToString("MMM d, yyyy");

            var url = GetReturnUrlInternal();
            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            var comment = ServiceLocator.JournalSearch.GetJournalComment(CommentIdentifier);
            if (comment?.LogbookIdentifier == null)
                RedirectToSearch();

            var journal = ServiceLocator.JournalSearch.GetJournal(comment.LogbookIdentifier.Value, x => x.JournalSetup);
            if (journal == null || journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier || journal.UserIdentifier != UserIdentifier)
                RedirectToSearch();

            JournalIdentifier = journal.JournalIdentifier;
            JournalSetupIdentifier = journal.JournalSetupIdentifier;

            var journalSetup = journal.JournalSetup;
            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier);

            Detail.SetInputValues(JournalSetupIdentifier, UserIdentifier, comment);

            CancelButton.NavigateUrl = GetReturnUrlInternal();

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text.Get(Identity.Language) ?? journalSetup.JournalSetupName);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, $"journalsetup={JournalSetupIdentifier}&panel=comments");

        private string GetReturnUrlInternal() => GetReturnUrl()
            .IfNullOrEmpty($"/ui/portal/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=comments");

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search", true);
    }
}