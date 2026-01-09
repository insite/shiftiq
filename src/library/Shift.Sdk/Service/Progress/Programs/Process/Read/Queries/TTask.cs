using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TTask
    {
        public Guid ObjectIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid TaskIdentifier { get; set; }

        public string ObjectType { get; set; }
        public string TaskCompletionRequirement { get; set; }
        public string TaskImage { get; set; }

        public bool TaskIsRequired { get; set; }
        public bool TaskIsPlanned { get; set; }

        public int? TaskLifetimeMonths { get; set; }
        public int TaskSequence { get; set; }

        public virtual TProgram Program { get; set; }
        public virtual ICollection<TTaskEnrollment> Enrollments { get; set; } = new HashSet<TTaskEnrollment>();
    }
}
