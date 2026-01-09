using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveQuiz : Query<QuizModel>
    {
        public Guid QuizIdentifier { get; set; }
    }
}