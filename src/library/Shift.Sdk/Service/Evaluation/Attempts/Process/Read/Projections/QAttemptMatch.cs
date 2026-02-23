using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Attempts.Read
{
    public class QAttemptMatch : IAttemptAnswer
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public string AnswerText { get; set; }
        public string MatchLeftText { get; set; }
        public string MatchRightText { get; set; }
        public int MatchSequence { get; set; }
        public int QuestionSequence { get; set; }
        public decimal MatchPoints { get; set; }

        [JsonIgnore]
        public string MatchLetter => Calculator.ToBase26(MatchSequence);

        [JsonIgnore]
        public bool HasAnswer => !string.IsNullOrEmpty(AnswerText);

        [JsonIgnore]
        public bool IsCorrect => AnswerText == MatchRightText;

        public virtual QAttempt Attempt { get; set; }

        #region IAttemptAnswer

        bool? IAttemptAnswer.IsSelected => !HasAnswer ? (bool?)null : IsCorrect;

        bool? IAttemptAnswer.IsTrue => true;

        decimal IAttemptAnswer.Points => MatchPoints;

        #endregion
    }
}
