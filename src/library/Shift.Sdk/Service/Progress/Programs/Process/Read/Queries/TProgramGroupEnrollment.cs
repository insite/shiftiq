using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class TProgramGroupEnrollment
    {
        public Guid ProgramGroupEnrollmentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }

        public virtual TProgram Program { get; set; }
        public virtual QGroup Group { get; set; }
    }
}
