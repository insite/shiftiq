using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttemptQuestion : Query<AttemptQuestionModel>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}