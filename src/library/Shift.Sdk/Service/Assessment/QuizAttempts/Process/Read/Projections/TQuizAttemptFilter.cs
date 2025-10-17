using System;

using Shift.Common;

namespace InSite.Application.QuizAttempts.Read
{
    [Serializable]
    public class TQuizAttemptFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? QuizIdentifier { get; set; }
        public Guid? LearnerIdentifier { get; set; }
        public string QuizType { get; set; }
        public string QuizNameContains { get; set; }
        public string LearnerNameContains { get; set; }
        public string LearnerEmailContains { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTimeOffset? AttemptStartedSince { get; set; }
        public DateTimeOffset? AttemptStartedBefore { get; set; }

        public TQuizAttemptFilter Clone()
        {
            return (TQuizAttemptFilter)MemberwiseClone();
        }
    }
}

