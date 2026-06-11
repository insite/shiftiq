using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Glossaries
{
    public class GlossaryTermUnlinked : Change
    {
        public Guid RelationshipId { get; set; }
        public Guid TermId { get; set; }

        public GlossaryTermUnlinked(Guid relationship, Guid term)
        {
            RelationshipId = relationship;
            TermId = term;
        }
    }
}
