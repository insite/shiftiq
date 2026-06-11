using System;

using Shift.Common;

namespace InSite.Application.Issues.Read
{
    [Serializable]
    public class QIssueAttachmentFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid? InputterUserIdentifier { get; set; }
        public Guid? IssueIdentifier { get; set; }
        public Guid? TopicUserIdentifier { get; set; }
        public Guid[] TopicUserIdentifiers { get; set; }
    }
}
