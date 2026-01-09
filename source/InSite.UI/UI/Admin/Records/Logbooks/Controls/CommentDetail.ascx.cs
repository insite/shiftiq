using System;
using System.Web.UI;

using InSite.Application.Contents.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class CommentDetail : UserControl
    {
        public bool SetDefaultInputValues(Guid journalSetupIdentifier, Guid userIdentifier, Guid? experienceId)
        {
            EntryField.Visible = false;

            return experienceId.HasValue
                ? SetupExperienceEntry(experienceId.Value, null)
                : SetupJournalEntry(journalSetupIdentifier, userIdentifier, null);
        }

        public bool SetInputValues(Guid journalSetupIdentifier, Guid userIdentifier, QComment comment)
        {
            EntryField.Visible = false;

            var isEntryLoaded = comment.LogbookExperienceIdentifier.HasValue
                ? SetupExperienceEntry(comment.LogbookExperienceIdentifier.Value, comment.CommentIdentifier)
                : SetupJournalEntry(journalSetupIdentifier, userIdentifier, comment.CommentIdentifier);

            if (!isEntryLoaded)
                return false;

            CommentText.Value = comment.CommentText;
            IsPrivate.SelectedValue = comment.CommentIsPrivate.ToString().ToLower();

            return true;
        }

        public void GetInputValues(QComment comment)
        {
            comment.CommentText = CommentText.Value;
            comment.CommentIsPrivate = bool.Parse(IsPrivate.SelectedValue);
        }

        private bool SetupExperienceEntry(Guid experienceId, Guid? commentId)
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(experienceId);
            if (experience == null)
                return false;

            EntryField.Visible = true;
            EntryName.Text = $"Entry #{experience.Sequence} added on {experience.ExperienceCreated.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone)}";

            CommentUpload.FolderPath = commentId.HasValue
                ? OrganizationRelativePath.JournalExperienceCommentChangePathTemplate.Format(experienceId, commentId.Value)
                : OrganizationRelativePath.JournalExperienceCommentCreatePathTemplate.Format(experienceId, Guid.NewGuid());

            return true;
        }

        private bool SetupJournalEntry(Guid journalSetupIdentifier, Guid userIdentifier, Guid? commentId)
        {
            CommentUpload.FolderPath = commentId.HasValue
                ? OrganizationRelativePath.JournalCommentChangePathTemplate.Format(journalSetupIdentifier, userIdentifier, commentId.Value)
                : OrganizationRelativePath.JournalCommentCreatePathTemplate.Format(journalSetupIdentifier, userIdentifier, Guid.NewGuid());

            return true;
        }
    }
}