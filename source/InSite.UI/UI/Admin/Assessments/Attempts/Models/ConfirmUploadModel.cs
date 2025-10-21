using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.File;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    [Serializable]
    internal class ConfirmUploadModel
    {
        #region Classes

        [Serializable]
        public class DatabaseAttemptInfo
        {
            public Guid FormIdentifier { get; set; }
            public Guid LearnerUserIdentifier { get; set; }
            public DateTimeOffset? AttemptSubmitted { get; set; }
            public decimal? AttemptScore { get; set; }
            public bool AttemptIsPassing { get; set; }
            public IEnumerable<DatabaseQuestionInfo> Questions { get; set; }

            public bool IsMatch(AttemptUploadAnswer attempt)
            {
                return FormIdentifier == attempt.FormIdentifier.Value
                    && LearnerUserIdentifier == attempt.LearnerUserIdentifier.Value
                    && AttemptSubmitted.Value.Date == attempt.AttemptGraded.Value.Date
                    && AttemptScore == attempt.Percent
                    && AttemptIsPassing == attempt.IsPassed
                    && Questions.Count() == attempt.Questions.Length
                    && Questions.OrderBy(q => q.QuestionSequence)
                                .Zip(
                                    attempt.Questions.OrderBy(q => q.Sequence),
                                    (q1, q2) => (q1.AnswerOptionSequence ?? 0) == q2.OptionIndex + 1)
                                .All(r => r);
            }
        }

        [Serializable]
        public class DatabaseQuestionInfo
        {
            public int QuestionSequence { get; set; }
            public int? AnswerOptionSequence { get; set; }
        }

        #endregion

        public AttemptUploadAnswer[] UploadAttempts { get; set; }
        public DatabaseAttemptInfo[] DatabaseAttempts { get; set; }
    }
}