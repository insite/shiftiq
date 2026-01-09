using System;
using System.Web.UI;

using InSite.Application.Journals.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class DeleteJournalComment : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid CommentIdentifier => Guid.TryParse(Request.QueryString["comment"], out var value) ? value : Guid.Empty;

        private Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var command = new DeleteComment(JournalIdentifier, CommentIdentifier);

            ServiceLocator.SendCommand(command);

            var url = GetParentUrl(null);

            HttpResponseHelper.Redirect(url);
        }

        private void RedirectToSearch()
        {
            HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");
        }

        private void LoadData()
        {
            var comment = ServiceLocator.JournalSearch.GetJournalComment(CommentIdentifier);
            if (comment?.LogbookIdentifier == null)
                RedirectToSearch();

            var journal = ServiceLocator.JournalSearch.GetJournal(comment.LogbookIdentifier.Value,
                x => x.User,
                x => x.JournalSetup,
                x => x.JournalSetup.Event,
                x => x.JournalSetup.Achievement,
                x => x.JournalSetup.Framework);

            if (journal == null || journal.JournalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                RedirectToSearch();

            if (ServiceLocator.JournalSearch.GetJournalSetupUser(journal.JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator) == null)
                RedirectToSearch();

            JournalIdentifier = journal.JournalIdentifier;

            var student = PersonSearch.Select(Organization.Identifier, journal.UserIdentifier, x => x.User);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{journal.JournalSetup.JournalSetupName} <span class='form-text'>{journal.User.UserFullName}</span>");

            UserName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={student.User.UserIdentifier}\">{student.User.FullName}</a>";
            UserEmail.Text = $"<a href='mailto:{student.User.Email}'>{student.User.Email}</a>";

            LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={journal.JournalSetupIdentifier}\">{journal.JournalSetup.JournalSetupName}</a>";
            var content = ServiceLocator.ContentSearch.GetBlock(journal.JournalSetupIdentifier, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default;
            LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";

            Posted.Text = GetLocalTime(comment.CommentPosted);
            Text.Text = Markdown.ToHtml(comment.CommentText);

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        protected static string GetLocalTime(object obj)
        {
            if (obj == null)
                return null;

            var date = (DateTimeOffset)obj;
            return TimeZones.Format(date, User.TimeZone, true);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);
    }
}