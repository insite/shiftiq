using System;

namespace Shift.Contract
{
    public partial class AttemptMatchModel
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public string AnswerText { get; set; }
        public string MatchLeftText { get; set; }
        public string MatchRightText { get; set; }

        public int MatchSequence { get; set; }
        public int QuestionSequence { get; set; }

        public decimal MatchPoints { get; set; }
    }
}