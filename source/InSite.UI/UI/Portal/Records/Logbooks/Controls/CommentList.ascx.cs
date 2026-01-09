using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    public partial class CommentList : UserControl
    {
        private List<QExperience> _experiences;
        private ReturnUrl _returnUrl = null;

        public void LoadData(Guid journalSetupIdentifier, Guid? experienceIdentifier, List<QExperience> experiences)
        {
            _experiences = experiences;
            _returnUrl = new ReturnUrl();

            var commentUrl = experienceIdentifier == null
                ? $"/ui/portal/records/logbooks/create-comment?journalsetup={journalSetupIdentifier}"
                : $"/ui/portal/records/logbooks/create-comment?journalsetup={journalSetupIdentifier}&experience={experienceIdentifier}";

            AddCommentLink.NavigateUrl = _returnUrl.GetRedirectUrl(commentUrl, "panel=comments");

            var user = CurrentSessionState.Identity.User;
            var journal = ServiceLocator.JournalSearch.GetJournal(journalSetupIdentifier, user.UserIdentifier);
            var hasJournal = journal != null;

            Repeater.Visible = hasJournal;

            if (!hasJournal)
                return;

            var comments = ServiceLocator.JournalSearch
                .GetJournalComments(journal.JournalIdentifier)
                .Where(x => !x.CommentIsPrivate || x.AuthorUserIdentifier == user.UserIdentifier)
                .Select(x =>
                {
                    var posted = TimeZoneInfo.ConvertTime(x.CommentPosted, user.TimeZone);
                    return new
                    {
                        x.LogbookIdentifier,
                        x.CommentIdentifier,
                        x.ContainerIdentifier,
                        AuthorIdentifier = x.AuthorUserIdentifier,
                        AuthorName = x.AuthorUserName,
                        PostedOn = $"posted {posted.Humanize()}",
                        Text = Markdown.ToHtml(x.CommentText)
                    };
                })
                .ToList();

            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.DataSource = comments;
            Repeater.DataBind();
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var logbookIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "LogbookIdentifier");
            var commentIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "CommentIdentifier");
            var authorIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "AuthorIdentifier");
            var containerIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "ContainerIdentifier");
            var canChange = authorIdentifier == CurrentSessionState.Identity.User.UserIdentifier;

            var changeLink = (HtmlAnchor)e.Item.FindControl("ChangeLink");
            changeLink.Visible = canChange;
            changeLink.HRef = _returnUrl
                .GetRedirectUrl($"/ui/portal/records/logbooks/change-comment?comment={commentIdentifier}", "panel=comments");

            var deleteLink = (HtmlAnchor)e.Item.FindControl("DeleteLink");
            deleteLink.HRef = _returnUrl
                .GetRedirectUrl($"/ui/portal/records/logbooks/delete-comment?comment={commentIdentifier}", "panel=comments");
            deleteLink.Visible = canChange;

            var isExperience = logbookIdentifier != containerIdentifier;

            var subjectNameLiteral = (Literal)e.Item.FindControl("SubjectName");
            subjectNameLiteral.Visible = isExperience;

            if (isExperience)
            {
                var experience = _experiences.Find(x => x.ExperienceIdentifier == containerIdentifier);
                var entryName = $"Entry #{experience.Sequence} added on {experience.ExperienceCreated.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone)}";

                subjectNameLiteral.Text = entryName;
            }
        }
    }
}