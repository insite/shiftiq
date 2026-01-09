using System;

namespace Shift.Contract
{
    public class DeleteAttemptQuestion
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}