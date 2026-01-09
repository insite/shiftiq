using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IQuizAttemptCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? LearnerIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? QuizGradebookIdentifier { get; set; }
        Guid? QuizIdentifier { get; set; }

        bool? AttemptIsPassing { get; set; }

        string AttemptData { get; set; }
        string QuizData { get; set; }
        string QuizName { get; set; }
        string QuizType { get; set; }

        int? AttemptCharsPerMin { get; set; }
        int? AttemptKeystrokesPerHour { get; set; }
        int? AttemptMistakes { get; set; }
        int? AttemptWordsPerMin { get; set; }
        int? QuizPassingKph { get; set; }
        int? QuizPassingWpm { get; set; }
        int? QuizTimeLimit { get; set; }

        decimal? AttemptAccuracy { get; set; }
        decimal? AttemptDuration { get; set; }
        decimal? AttemptScore { get; set; }
        decimal? AttemptSpeed { get; set; }
        decimal? QuizPassingAccuracy { get; set; }

        DateTimeOffset? AttemptCompleted { get; set; }
        DateTimeOffset? AttemptCreated { get; set; }
        DateTimeOffset? AttemptStarted { get; set; }
    }
}