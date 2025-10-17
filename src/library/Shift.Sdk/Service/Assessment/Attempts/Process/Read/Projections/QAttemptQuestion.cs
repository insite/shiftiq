﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Attempts.Read
{
    public class QAttemptQuestion
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid? CompetencyAreaIdentifier { get; set; }
        public Guid? CompetencyItemIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid? ParentQuestionIdentifier { get; set; }
        public string CompetencyAreaCode { get; set; }
        public string CompetencyAreaLabel { get; set; }
        public string CompetencyAreaTitle { get; set; }
        public string CompetencyItemCode { get; set; }
        public string AnswerText { get; set; }
        public string QuestionMatchDistractors { get; set; }
        public string QuestionText { get; set; }
        public string CompetencyItemLabel { get; set; }
        public string CompetencyItemTitle { get; set; }
        public string QuestionCalculationMethod { get; set; }
        public string QuestionType { get; set; }
        public int? AnswerOptionKey { get; set; }
        public int? AnswerOptionSequence { get; set; }
        public int QuestionSequence { get; set; }
        public decimal? AnswerPoints { get; set; }
        public decimal? QuestionCutScore { get; set; }
        public decimal? QuestionPoints { get; set; }
        public int? PinLimit { get; set; }
        public string HotspotImage { get; set; }
        public bool? ShowShapes { get; set; }
        public int? AnswerTimeLimit { get; set; }
        public int? AnswerAttemptLimit { get; set; }
        public Guid? AnswerFileIdentifier { get; set; }
        public Guid? AnswerSolutionIdentifier { get; set; }
        public string QuestionTopLabel { get; set; }
        public string QuestionBottomLabel { get; set; }
        public int? SectionIndex { get; set; }
        public int? AnswerRequestAttempt { get; set; }
        public int? AnswerSubmitAttempt { get; set; }
        public int? QuestionNumber { get; set; }
        public string RubricRatingPoints { get; set; }

        public virtual QAttempt Attempt { get; set; }

        public void ClearAnswer()
        {
            AnswerOptionKey = null;
            AnswerOptionSequence = null;
            AnswerPoints = null;
            AnswerSolutionIdentifier = null;
        }

        public void SetMatchDistractors(IEnumerable<string> value)
        {
            QuestionMatchDistractors = JsonConvert.SerializeObject(value.EmptyIfNull()).NullIf("[]");
        }

        public string[] GetMatchDistractors()
        {
            return QuestionMatchDistractors.IsEmpty()
                ? new string[0] : JsonConvert.DeserializeObject<string[]>(QuestionMatchDistractors);
        }
    }
}
