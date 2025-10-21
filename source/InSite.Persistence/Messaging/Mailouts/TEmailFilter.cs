using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TEmailFilter : Filter
    {
        public string EmailSubject { get; set; }
        public string EmailFromName { get; set; }
        public string EmailFromEmail { get; set; }
        public string EmailTo { get; set; }

        public bool? DeliveryIsSuccessful { get; set; }

        public string StatusCode { get; set; }
        public string StatusId { get; set; }
        public string StatusMessage { get; set; }

        public Guid? MessageIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
    }
}
