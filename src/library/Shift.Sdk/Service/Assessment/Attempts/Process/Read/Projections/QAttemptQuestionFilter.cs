using System;

using Shift.Common;

namespace InSite.Application.Attempts.Read
{
    [Serializable]
    public class QAttemptQuestionFilter : Filter
    {
        public Guid? BankIdentifier { get; set; }
        public Guid? FormIdentifier { get; set; }
        public Guid? QuestionIdentifier { get; set; }
        public Guid? LearnerUserIdentifier { get; set; }
    }
}
