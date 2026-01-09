using System;

namespace Shift.Contract
{
    public class ModifyQuiz
    {
        public Guid GradebookIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid QuizIdentifier { get; set; }

        public string QuizData { get; set; }
        public string QuizName { get; set; }
        public string QuizType { get; set; }

        public int AttemptLimit { get; set; }
        public int PassingKph { get; set; }
        public int PassingWpm { get; set; }
        public int TimeLimit { get; set; }

        public decimal? MaximumPoints { get; set; }
        public decimal PassingAccuracy { get; set; }
        public decimal? PassingPoints { get; set; }
        public decimal? PassingScore { get; set; }
    }
}