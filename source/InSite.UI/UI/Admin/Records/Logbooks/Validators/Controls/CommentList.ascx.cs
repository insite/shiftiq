using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Records.Validators.Controls
{
    public partial class CommentList : BaseUserControl
    {
        private List<QExperience> _experiences;
        private ReturnUrl _returnUrl;

        public void LoadData(
            Guid journalSetupIdentifier,
            Guid userIdentifier,
            Guid? journalIdentifier,
            List<QExperience> experiences,
            Guid? experienceIdentifier
            )
        {
            var commentUrl = experienceIdentifier == null
                ? $"/ui/admin/records/logbooks/validators/create-journal-comment?journalSetup={journalSetupIdentifier}&user={userIdentifier}"
                : $"/ui/admin/records/logbooks/validators/create-journal-comment?experience={experienceIdentifier}";

            _returnUrl = new ReturnUrl();

            AddCommentLink.NavigateUrl = _returnUrl.GetRedirectUrl(commentUrl, "panel=comments");

            if (journalIdentifier == null)
                return;

            var comments = ServiceLocator.JournalSearch
                .GetJournalComments(journalIdentifier.Value)
                .Where(x => experienceIdentifier == null || x.LogbookExperienceIdentifier == experienceIdentifier)
                .Select(x =>
                {

                    var posted = TimeZoneInfo.ConvertTime(x.CommentPosted, User.TimeZone);

                    return new
                    {
                        JournalIdentifier = journalIdentifier,
                        x.CommentIdentifier,
                        x.ContainerIdentifier,
                        x.CommentIsPrivate,
                        AuthorIdentifier = x.AuthorUserIdentifier,
                        AuthorName = x.AuthorUserName,
                        PostedOn = $"posted {posted.Humanize()}",
                        Text = Markdown.ToHtml(x.CommentText),
                        IsPrivate = x.CommentIsPrivate
                    };
                })
                .ToList();

            _experiences = experiences;

            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.DataSource = comments;
            Repeater.DataBind();
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var commentIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "CommentIdentifier");
            var authorIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "AuthorIdentifier");
            var journalIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "JournalIdentifier");
            var containerIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "ContainerIdentifier");

            var changeLink = (HtmlAnchor)e.Item.FindControl("ChangeLink");
            changeLink.Visible = authorIdentifier == User.UserIdentifier;
            changeLink.HRef = _returnUrl
                .GetRedirectUrl($"/ui/admin/records/logbooks/validators/change-journal-comment?comment={commentIdentifier}", "panel=comments");

            var deleteLink = (HtmlAnchor)e.Item.FindControl("DeleteLink");
            deleteLink.HRef = _returnUrl
                .GetRedirectUrl($"/admin/records/logbooks/validators/delete-journal-comment?comment={commentIdentifier}", "panel=comments");

            var isExperience = journalIdentifier != containerIdentifier;

            var subjectNameLiteral = e.Item.FindControl("SubjectName");
            subjectNameLiteral.Visible = isExperience;

            if (isExperience)
            {
                var experience = _experiences.Find(x => x.ExperienceIdentifier == containerIdentifier);
                var entryName = $"Entry #{experience.Sequence} added on {experience.ExperienceCreated.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone)}";

                ((ITextControl)subjectNameLiteral).Text = entryName;
            }
        }
    }
}