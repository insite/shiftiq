using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveQuizAttempt : Query<QuizAttemptModel>
    {
        public Guid AttemptIdentifier { get; set; }
    }
}