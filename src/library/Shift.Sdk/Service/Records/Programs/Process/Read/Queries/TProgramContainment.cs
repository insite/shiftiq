using System;

namespace InSite.Application.Records.Read
{
    public class TProgramContainment
    {
        public Guid ParentProgramIdentifier { get; set; }
        public Guid ChildProgramIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public int ChildSequence { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
