using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Attempts.Read
{
    public class QAttemptOption : IAttemptAnswer
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public string OptionText { get; set; }

        public bool? AnswerIsSelected { get; set; }
        public bool? OptionIsTrue { get; set; }

        public int OptionKey { get; set; }
        public int OptionSequence { get; set; }
        public int QuestionSequence { get; set; }
        public int? OptionAnswerSequence { get; set; }

        public decimal? OptionCutScore { get; set; }
        public decimal OptionPoints { get; set; }

        public string OptionShape { get; set; }

        public Guid? CompetencyAreaIdentifier { get; set; }
        public string CompetencyAreaCode { get; set; }
        public string CompetencyAreaLabel { get; set; }
        public string CompetencyAreaTitle { get; set; }

        public Guid? CompetencyItemIdentifier { get; set; }
        public string CompetencyItemCode { get; set; }
        public string CompetencyItemLabel { get; set; }
        public string CompetencyItemTitle { get; set; }

        [JsonIgnore]
        public string OptionLetter => Calculator.ToBase26(OptionSequence);

        public virtual QAttempt Attempt { get; set; }

        #region IAttemptAnswer

        bool? IAttemptAnswer.IsSelected => AnswerIsSelected;

        bool? IAttemptAnswer.IsTrue => OptionIsTrue;

        decimal IAttemptAnswer.Points => OptionPoints;

        #endregion
    }
}
