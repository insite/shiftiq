using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardContainment
    {
        public Guid ParentStandardIdentifier { get; set; }
        public Guid ChildStandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public int ChildSequence { get; set; }
        public decimal? CreditHours { get; set; }
        public string CreditType { get; set; }
    }
}
