using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchAttemptOptions : Query<IEnumerable<AttemptOptionMatch>>, IAttemptOptionCriteria
    {
        public Guid? CompetencyAreaIdentifier { get; set; }
        public Guid? CompetencyItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public bool? AnswerIsSelected { get; set; }
        public bool? OptionIsTrue { get; set; }

        public string CompetencyAreaCode { get; set; }
        public string CompetencyAreaLabel { get; set; }
        public string CompetencyAreaTitle { get; set; }
        public string CompetencyItemCode { get; set; }
        public string CompetencyItemLabel { get; set; }
        public string CompetencyItemTitle { get; set; }
        public string OptionShape { get; set; }
        public string OptionText { get; set; }

        public int? OptionAnswerSequence { get; set; }
        public int? OptionSequence { get; set; }
        public int? QuestionSequence { get; set; }

        public decimal? OptionCutScore { get; set; }
        public decimal? OptionPoints { get; set; }
    }
}