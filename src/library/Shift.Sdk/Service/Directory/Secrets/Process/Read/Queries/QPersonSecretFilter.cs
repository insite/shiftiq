using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QPersonSecretFilter : Filter
    {
        public Guid? UserIdentifier { get; set; }
        public string UserName { get; set; }
        public string OrganizationCode { get; set; }
        public DateTimeOffset? TokenIssuedSince { get; set; }
        public DateTimeOffset? TokenIssuedBefore { get; set; }
        public DateTimeOffset? TokenExpiredSince { get; set; }
        public DateTimeOffset? TokenExpiredBefore { get; set; }
    }
}
