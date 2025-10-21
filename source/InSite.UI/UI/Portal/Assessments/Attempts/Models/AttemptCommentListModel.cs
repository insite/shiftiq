using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Sdk.UI;

namespace InSite.Portal.Assessments.Attempts.Models
{
    public class AttemptCommentListModel
    {
        public IEnumerable<AttemptCommentModel> Comments { get; private set; }

        private AttemptCommentListModel()
        {

        }

        public static AttemptCommentListModel Create(Guid attemptId, int? sectionIndex)
        {
            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(attemptId, sectionIndex).ToDictionary(x => x.QuestionIdentifier, x => x);

            var comments = ServiceLocator.AttemptSearch.GetVAttemptComments(attemptId).Where(x =>
                x.AssessmentQuestionIdentifier != null
                && questions.ContainsKey(x.AssessmentQuestionIdentifier.Value)
                ).Select(x =>
            {
                var question = questions[x.AssessmentQuestionIdentifier.Value];

                return new AttemptCommentModel
                {
                    QuestionSequence = question.QuestionSequence,
                    QuestionTitle = question.QuestionText,
                    CommentText = x.CommentText,
                    CommentPosted = x.CommentPosted
                };
            });

            return new AttemptCommentListModel
            {
                Comments = comments.OrderByDescending(x => x.CommentPosted)
            };
        }
    }
}