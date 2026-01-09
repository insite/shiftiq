using System;

using Shift.Common;

namespace InSite.Application.Issues.Read
{
    public class QIssueUserFilter : Filter
    {
        public Guid? IssueOrganizationIdentifier { get; set; }
        public Guid? Issuedentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public string IssueRole { get; set; }

        public QIssueFilter Clone()
        {
            return (QIssueFilter)MemberwiseClone();
        }
    }
}
