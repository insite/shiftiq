using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertQuiz : Query<bool>
    {
        public Guid QuizIdentifier { get; set; }
    }
}