using System;

using InSite.Domain.Organizations;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class EmailFilter : Filter
    {
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTo { get; set; }

        public string SenderEmail { get; set; }
        public string SenderName { get; set; }

        public string ToName { get; set; }

        public string StatusMessage { get; set; }
        public string StatusCode { get; set; }

        public string OrganizationName { get; set; }

        public bool? DeliverySuccessful { get; set; }
        public DateTimeOffset? DeliveredSince { get; set; }
        public DateTimeOffset? DeliveredBefore { get; set; }

        public Guid OrganizationIdentifier { get; set; }
        public Guid EmailIdentifier { get; set; }

        public OrganizationList Organizations { get; set; }
    }
}
