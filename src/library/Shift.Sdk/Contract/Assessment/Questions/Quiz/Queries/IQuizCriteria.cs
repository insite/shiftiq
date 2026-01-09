using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IQuizCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? GradebookIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }

        string QuizData { get; set; }
        string QuizName { get; set; }
        string QuizType { get; set; }

        int? AttemptLimit { get; set; }
        int? PassingKph { get; set; }
        int? PassingWpm { get; set; }
        int? TimeLimit { get; set; }

        decimal? MaximumPoints { get; set; }
        decimal? PassingAccuracy { get; set; }
        decimal? PassingPoints { get; set; }
        decimal? PassingScore { get; set; }
    }
}