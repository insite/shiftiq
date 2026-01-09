using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchQuizAttempts : Query<IEnumerable<QuizAttemptMatch>>, IQuizAttemptCriteria
    {
        public Guid? LearnerIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? QuizGradebookIdentifier { get; set; }
        public Guid? QuizIdentifier { get; set; }

        public bool? AttemptIsPassing { get; set; }

        public string AttemptData { get; set; }
        public string QuizData { get; set; }
        public string QuizName { get; set; }
        public string QuizType { get; set; }

        public int? AttemptCharsPerMin { get; set; }
        public int? AttemptKeystrokesPerHour { get; set; }
        public int? AttemptMistakes { get; set; }
        public int? AttemptWordsPerMin { get; set; }
        public int? QuizPassingKph { get; set; }
        public int? QuizPassingWpm { get; set; }
        public int? QuizTimeLimit { get; set; }

        public decimal? AttemptAccuracy { get; set; }
        public decimal? AttemptDuration { get; set; }
        public decimal? AttemptScore { get; set; }
        public decimal? AttemptSpeed { get; set; }
        public decimal? QuizPassingAccuracy { get; set; }

        public DateTimeOffset? AttemptCompleted { get; set; }
        public DateTimeOffset? AttemptCreated { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
    }
}