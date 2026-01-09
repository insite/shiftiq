using System;

using Shift.Common;

namespace InSite.Application.Sites.Read
{
    [Serializable]
    public class QSiteFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }

        public string Domain { get; set; }
        public string Title { get; set; }
        public string Keyword { get; set; }

        public DateTimeOffset? LastModifiedSince { get; set; }
        public DateTimeOffset? LastModifiedBefore { get; set; }
    }
}