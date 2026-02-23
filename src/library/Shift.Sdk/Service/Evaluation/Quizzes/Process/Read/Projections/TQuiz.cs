using System;
using System.Collections.Generic;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Records.Read;

namespace InSite.Application.Quizzes.Read
{
    public class TQuiz
    {
        public const int MaxQuizDataLength = 4000;

        public Guid QuizIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }

        public string QuizType { get; set; }
        public string QuizName { get; set; }
        public string QuizData { get; set; }

        public int TimeLimit { get; set; }
        public int AttemptLimit { get; set; }

        public decimal? PassingScore { get; set; }
        public decimal? MaximumPoints { get; set; }
        public decimal? PassingPoints { get; set; }

        public decimal PassingAccuracy { get; set; }
        public int PassingWpm { get; set; }
        public int PassingKph { get; set; }

        public virtual QGradebook Gradebook { get; set; }

        public virtual ICollection<TQuizAttempt> Attempts { get; set; }

        public TQuiz()
        {
            Attempts = new HashSet<TQuizAttempt>();
        }
    }
}
