using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Glossaries.Write
{
    public class UnlinkGlossaryTerm : Command
    {
        public Guid RelationshipIdentifier { get; set; }
        public Guid TermIdentifier { get; set; }

        public UnlinkGlossaryTerm(Guid glossary, Guid relationship, Guid term)
        {
            AggregateIdentifier = glossary;
            RelationshipIdentifier = relationship;
            TermIdentifier = term;
        }
    }
}
