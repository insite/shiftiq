using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TProgramEnrollment
    {
        public Guid? CreatedBy { get; set; }
        public DateTimeOffset? Created { get; set; }

        public Guid EnrollmentIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }

        public DateTimeOffset? ProgressStarted { get; set; }
        public DateTimeOffset? ProgressCompleted { get; set; }

        public int MessageStalledSentCount { get; set; }
        public int MessageCompletedSentCount { get; set; }


        public virtual TProgram Program { get; set; }
        public virtual QUser LearnerUser { get; set; }
    }
}
