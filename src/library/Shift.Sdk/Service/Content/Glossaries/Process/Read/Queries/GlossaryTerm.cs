using System;
using System.Collections.Generic;

namespace InSite.Application.Glossaries.Read
{
    [Serializable]
    public class QGlossaryTerm
    {
        public Guid GlossaryIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid TermIdentifier { get; set; }
        public string TermName { get; set; }
        public string TermStatus { get; set; }

        public DateTimeOffset Proposed { get; set; }
        public string ProposedBy { get; set; }
        public DateTimeOffset? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTimeOffset? LastRevised { get; set; }
        public string LastRevisedBy { get; set; }
        public int RevisionCount { get; set; }

        public virtual ICollection<QGlossaryTermContent> TermContents { get; set; } = new HashSet<QGlossaryTermContent>();
    }
}
