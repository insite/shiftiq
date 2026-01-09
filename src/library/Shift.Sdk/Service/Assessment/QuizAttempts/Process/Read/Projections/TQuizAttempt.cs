using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Quizzes.Read;

namespace InSite.Application.QuizAttempts.Read
{
    [Serializable]
    public class TQuizAttempt
    {
        public const int MaxAttemptDataLength = 4000;
        public const int MaxAttemptResultLength = 4000;

        public Guid AttemptIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid QuizIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }

        public DateTimeOffset AttemptCreated { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptCompleted { get; set; }

        public Guid QuizGradebookIdentifier { get; set; }
        public string QuizType { get; set; }
        public string QuizName { get; set; }
        public string QuizData { get; set; }
        public int QuizTimeLimit { get; set; }
        public decimal QuizPassingAccuracy { get; set; }
        public int QuizPassingWpm { get; set; }
        public int QuizPassingKph { get; set; }

        public string AttemptData { get; set; }
        public bool? AttemptIsPassing { get; set; }
        public decimal? AttemptScore { get; set; }
        public decimal? AttemptDuration { get; set; }
        public int? AttemptMistakes { get; set; }
        public decimal? AttemptAccuracy { get; set; }
        public int? AttemptCharsPerMin { get; set; }
        public int? AttemptWordsPerMin { get; set; }
        public int? AttemptKeystrokesPerHour { get; set; }
        public decimal? AttemptSpeed { get; set; }

        public virtual TQuiz Quiz { get; set; }
        public virtual VPerson LearnerPerson { get; set; }
        public virtual VUser LearnerUser { get; set; }
    }
}
