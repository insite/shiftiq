using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptOptionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? CompetencyAreaIdentifier { get; set; }
        Guid? CompetencyItemIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }

        bool? AnswerIsSelected { get; set; }
        bool? OptionIsTrue { get; set; }

        string CompetencyAreaCode { get; set; }
        string CompetencyAreaLabel { get; set; }
        string CompetencyAreaTitle { get; set; }
        string CompetencyItemCode { get; set; }
        string CompetencyItemLabel { get; set; }
        string CompetencyItemTitle { get; set; }
        string OptionShape { get; set; }
        string OptionText { get; set; }

        int? OptionAnswerSequence { get; set; }
        int? OptionSequence { get; set; }
        int? QuestionSequence { get; set; }

        decimal? OptionCutScore { get; set; }
        decimal? OptionPoints { get; set; }
    }
}