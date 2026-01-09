using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class ApiRequestFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public DateTimeOffset? RequestStartedSince { get; set; }
        public DateTimeOffset? RequestStartedBefore { get; set; }
        public string RequestData { get; set; }
        public string RequestUri { get; set; }
        public bool? RequestIsIncoming { get; set; }
    }
}
