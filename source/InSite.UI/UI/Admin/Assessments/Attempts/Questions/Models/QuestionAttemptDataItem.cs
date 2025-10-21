using System;

using InSite.Application.Attempts.Read;

using Shift.Common;

namespace InSite.Admin.Attempts.Questions.Models
{
    [Serializable]
    public class QuestionAttemptDataItem
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public string AttemptGraded { get; set; }
        public string AttemptCandidateName { get; set; }
        public int? AnswerOptionSequence { get; set; }
        public string AnswerText { get; set; }
        public decimal? AnswerPoints { get; set; }
        public decimal? QuestionPoints { get; set; }

        public QuestionAttemptDataItem(QAttemptQuestion entity)
        {
            var tz = CurrentSessionState.Identity.User.TimeZone;

            AttemptIdentifier = entity.AttemptIdentifier;
            QuestionIdentifier = entity.QuestionIdentifier;

            var attempt = entity.Attempt;
            AttemptGraded = attempt?.AttemptGraded.Format(tz, nullValue: string.Empty);
            AttemptCandidateName = entity.Attempt?.LearnerPerson?.UserFullName;
            AnswerOptionSequence = entity.AnswerOptionSequence;
            AnswerText = entity.AnswerText;
            AnswerPoints = entity.AnswerPoints;
            QuestionPoints = entity.QuestionPoints;
        }
    }
}