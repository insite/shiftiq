using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.Infrastructure;
using InSite.Portal.Assessments.Attempts.Models;

using Shift.Common;

namespace InSite.Portal.Assessments.Attempts.Controls
{
    public partial class CommentList : UserControl
    {
        private class DataItem
        {
            public string CommentText { get; set; }
            public string CommentPosted { get; set; }
            public int QuestionSequence { get; set; }
            public string QuestionTitle { get; set; }
        }

        public bool LoadData(Guid attemptId)
        {
            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (attempt == null || attempt.AssessorUserIdentifier != CurrentSessionState.Identity.User.UserIdentifier)
                return false;

            var sectionIndex = attempt.SectionsAsTabsEnabled && !attempt.TabNavigationEnabled
                ? attempt.ActiveSectionIndex ?? -1
                : (int?)null;

            var model = AttemptCommentListModel.Create(attemptId, sectionIndex);
            var hasComments = model != null && model.Comments.Any();

            CommentRepeater.Visible = hasComments;
            NoComments.Visible = !hasComments;

            if (hasComments)
            {
                var comments = model.Comments
                    .Select(x => new DataItem
                    {
                        CommentText = AssetCommentHelper.GetDescription(x.CommentText, null, null, null),
                        CommentPosted = AssetCommentHelper.ConvertToHtml_When(x.CommentPosted),
                        QuestionSequence = x.QuestionSequence,
                        QuestionTitle = x.QuestionTitle
                    })
                    .ToList();

                foreach (var comment in comments)
                    if (comment.QuestionTitle != null)
                        comment.QuestionTitle = StringHelper.Replace(Markdown.ToHtml(comment.QuestionTitle), "<img src", "<img class='img-responsive' src");

                CommentRepeater.DataSource = comments;
                CommentRepeater.DataBind();
            }

            return true;
        }
    }
}