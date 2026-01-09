using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Journals.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class CreateJournalComment : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private class JournalInfo
        {
            public QJournalSetup JournalSetup { get; set; }
            public VUser User { get; set; }
        }

        private Guid? JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalSetup"], out var value) ? (Guid?)value : null;
        private Guid? UserIdentifier => Guid.TryParse(Request.QueryString["user"], out var value) ? (Guid?)value : null;
        private Guid? ExperienceIdentifier => Guid.TryParse(Request.QueryString["experience"], out var value) ? value : (Guid?)null;

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
            var journalIdentifier = GetJournalIdentifier(commands);
            var commentIdentifier = UniqueIdentifier.Create();

            var subjectIdentifier = ExperienceIdentifier.HasValue
                ? ExperienceIdentifier.Value
                : journalIdentifier;

            var subjectType = ExperienceIdentifier.HasValue
                ? "Experience"
                : "Journal";

            commands.Add(new AddComment(
                journalIdentifier,
                commentIdentifier,
                User.UserIdentifier,
                subjectIdentifier,
                subjectType,
                entity.CommentText,
                DateTimeOffset.UtcNow,
                entity.CommentIsPrivate
            ));

            ServiceLocator.SendCommands(commands);

            var url = GetParentUrl(null);

            HttpResponseHelper.Redirect(url);
        }

        private Guid GetJournalIdentifier(List<Command> commands)
        {
            if (ExperienceIdentifier.HasValue)
                return ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier.Value).JournalIdentifier;

            var journal = ServiceLocator.JournalSearch.GetJournal(JournalSetupIdentifier.Value, UserIdentifier.Value);
            if (journal != null)
                return journal.JournalIdentifier;

            var journalIdentifier = UniqueIdentifier.Create();

            commands.Add(new CreateJournal(journalIdentifier, JournalSetupIdentifier.Value, UserIdentifier.Value));

            return journalIdentifier;
        }

        private void LoadData()
        {
            var journalInfo = GetJournalInfo();
            if (journalInfo == null
                || journalInfo.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || ServiceLocator.JournalSearch.GetJournalSetupUser(journalInfo.JournalSetup.JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator) == null
                )
            {
                RedirectToSearch();
                return;
            }

            PageHelper.AutoBindHeader(
                this, 
                qualifier: $"{journalInfo.JournalSetup.JournalSetupName} <span class='form-text'>{journalInfo.User.UserFullName}</span>");

            var journalSetupIdentifier = journalInfo.JournalSetup.JournalSetupIdentifier;
            var userIdentifier = journalInfo.User.UserIdentifier;

            if (!Detail.SetDefaultInputValues(journalSetupIdentifier, userIdentifier, ExperienceIdentifier))
            {
                RedirectToSearch();
                return;
            }

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        private JournalInfo GetJournalInfo()
        {
            if (ExperienceIdentifier.HasValue)
            {
                var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier.Value, x => x.Journal.JournalSetup, x => x.Journal.User);
                return experience != null
                    ? new JournalInfo { JournalSetup = experience.Journal.JournalSetup, User = experience.Journal.User }
                    : null;
            }

            if (JournalSetupIdentifier == null || UserIdentifier == null)
                return null;

            var journalSetupUser = ServiceLocator.JournalSearch.GetJournalSetupUser(
                JournalSetupIdentifier.Value,
                UserIdentifier.Value,
                JournalSetupUserRole.Learner,
                x => x.User
            );

            if (journalSetupUser == null)
                return null;

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier.Value);

            return new JournalInfo { JournalSetup = journalSetup, User = journalSetupUser.User };
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");
    }
}
