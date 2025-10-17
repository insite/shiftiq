using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Glossaries.Read
{
    [Serializable]
    public class GlossaryTermFilter : Filter
    {
        public Guid? GlossaryIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string TermIdentifier { get; set; }
        public string TermName { get; set; }
        public string TermTitle { get; set; }
        public string TermDefinition { get; set; }
        public string TermKeyword { get; set; }
        public string TermStatus { get; set; }
        public bool? IsTranslated { get; set; }

        public List<Guid> ExcludeTermIdentifiers { get; set; }

        public int? RevisionCountFrom { get; set; }
        public int? RevisionCountThru { get; set; }

        public string ProposedBy { get; set; }
        public DateTimeOffset? ProposedSince { get; set; }
        public DateTimeOffset? ProposedBefore { get; set; }

        public string ApprovedBy { get; set; }
        public DateTimeOffset? ApprovedSince { get; set; }
        public DateTimeOffset? ApprovedBefore { get; set; }

        public string LastRevisedBy { get; set; }
        public DateTimeOffset? LastRevisedSince { get; set; }
        public DateTimeOffset? LastRevisedBefore { get; set; }

        public GlossaryTermFilter Clone()
        {
            return (GlossaryTermFilter)MemberwiseClone();
        }
    }
}
