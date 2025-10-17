using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardValidationLog
    {
        public Guid LogIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid StandardValidationIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? AuthorUserIdentifier { get; set; }
        public string LogComment { get; set; }
        public DateTimeOffset LogPosted { get; set; }
        public string LogStatus { get; set; }
    }
}
