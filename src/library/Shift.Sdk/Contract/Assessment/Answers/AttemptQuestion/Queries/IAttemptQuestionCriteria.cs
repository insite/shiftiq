using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptQuestionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? AnswerFileIdentifier { get; set; }
        Guid? AnswerSolutionIdentifier { get; set; }
        Guid? CompetencyAreaIdentifier { get; set; }
        Guid? CompetencyItemIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? ParentQuestionIdentifier { get; set; }

        bool? ShowShapes { get; set; }

        string AnswerText { get; set; }
        string CompetencyAreaCode { get; set; }
        string CompetencyAreaLabel { get; set; }
        string CompetencyAreaTitle { get; set; }
        string CompetencyItemCode { get; set; }
        string CompetencyItemLabel { get; set; }
        string CompetencyItemTitle { get; set; }
        string HotspotImage { get; set; }
        string QuestionBottomLabel { get; set; }
        string QuestionCalculationMethod { get; set; }
        string QuestionMatchDistractors { get; set; }
        string QuestionText { get; set; }
        string QuestionTopLabel { get; set; }
        string QuestionType { get; set; }
        string RubricRatingPoints { get; set; }

        int? AnswerAttemptLimit { get; set; }
        int? AnswerOptionKey { get; set; }
        int? AnswerOptionSequence { get; set; }
        int? AnswerRequestAttempt { get; set; }
        int? AnswerSubmitAttempt { get; set; }
        int? AnswerTimeLimit { get; set; }
        int? PinLimit { get; set; }
        int? QuestionNumber { get; set; }
        int? QuestionSequence { get; set; }
        int? SectionIndex { get; set; }

        decimal? AnswerPoints { get; set; }
        decimal? QuestionCutScore { get; set; }
        decimal? QuestionPoints { get; set; }
    }
}