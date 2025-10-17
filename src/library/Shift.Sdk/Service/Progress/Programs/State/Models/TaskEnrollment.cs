using System;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TaskEnrollment
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public Guid TaskIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
    }
}
