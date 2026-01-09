using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class AnalysisCommentTabContent : BaseUserControl
    {
        public int LoadData(Question question, DateTimeOffsetRange posted)
        {
            var adminComments = new List<Comment>();
            var candidateComments = new List<Comment>();

            var comments = question.Comments.AsQueryable();

            if (posted != null)
            {
                if (posted.Since.HasValue)
                    comments = comments.Where(x => x.Posted >= posted.Since.Value);

                if (posted.Before.HasValue)
                    comments = comments.Where(x => x.Posted < posted.Before.Value);
            }

            var feedback = ServiceLocator.AttemptSearch.SelectQuestionFeedbackForAnalysis(question.Identifier);

            foreach (var comment in comments)
            {
                bool shouldSkip = feedback.Any(f =>
                f.CommentIdentifier == comment.Identifier &&
                f.IsSameAssessor &&
                f.IsFormThirdPartyAssessmentEnabled);

                if (shouldSkip)
                    continue;

                if (comment.AuthorRole == CommentAuthorType.Administrator)
                    adminComments.Add(comment);
                else if (comment.AuthorRole == CommentAuthorType.Candidate)
                    candidateComments.Add(comment);
            }

            var hasAdminComments = adminComments.Count > 0;
            var hasCandidateComments = candidateComments.Count > 0;

            var returnUrl = new ReturnUrl();

            AdministratorColumn.Visible = hasAdminComments;
            AdministratorRepeater.LoadData(question.Set.Bank.Identifier, adminComments, returnUrl);

            CandidateColumn.Visible = hasCandidateComments;
            CandidateRepeater.LoadData(question.Set.Bank.Identifier, candidateComments, returnUrl);

            if (!hasAdminComments && !hasCandidateComments)
                MessageLiteral.Text = "<p>There are no comments posted to this question.</p>";

            return adminComments.Count + candidateComments.Count;
        }
    }
}