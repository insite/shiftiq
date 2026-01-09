using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Controls
{
    public partial class LogEntryComments : BaseUserControl
    {
        private List<QExperience> _experiences;

        public bool LoadData(QJournal journal, Guid? experienceIdentifier)
        {
            var journalIdentifier = journal.JournalIdentifier;

            var comments = ServiceLocator.JournalSearch
                .GetJournalComments(journalIdentifier)
                .Where(x => x.LogbookExperienceIdentifier == experienceIdentifier)
                .Select(x =>
                {
                    var posted = TimeZoneInfo.ConvertTime(x.CommentPosted, User.TimeZone);
                    return new
                    {
                        JournalIdentifier = journalIdentifier,
                        x.CommentIdentifier,
                        x.ContainerIdentifier,
                        x.ContainerDescription,
                        x.CommentIsPrivate,
                        AuthorIdentifier = x.AuthorUserIdentifier,
                        AuthorName = x.AuthorUserName,
                        PostedOn = $"posted {GetLocalTime(posted)}",
                        Text = Markdown.ToHtml(x.CommentText)
                    };
                })
                .ToList();

            _experiences = journal.Experiences.ToList();

            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.DataSource = comments;
            Repeater.DataBind();

            return comments.Count > 0;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var commentIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "CommentIdentifier");
            var authorIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "AuthorIdentifier");
            var journalIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "JournalIdentifier");
            var containerIdentifier = (Guid)DataBinder.Eval(e.Item.DataItem, "ContainerIdentifier");

            var isExperience = journalIdentifier != containerIdentifier;

            var subjectNameLiteral = (System.Web.UI.WebControls.Literal)e.Item.FindControl("SubjectName");
            subjectNameLiteral.Visible = isExperience;

            if (isExperience)
            {
                var experience = _experiences.Find(x => x.ExperienceIdentifier == containerIdentifier);
                var entryName = $"Entry #{experience.Sequence} added on {experience.ExperienceCreated.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone)}";

                subjectNameLiteral.Text = entryName;
            }
        }

        private static string GetLocalTime(DateTimeOffset? date)
            => date != null ? TimeZones.Format(date.Value, User.TimeZone, true) : null;
    }
}