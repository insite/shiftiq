using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardCategory
    {
        public Guid CategoryIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public int? ClassificationSequence { get; set; }
    }
}
