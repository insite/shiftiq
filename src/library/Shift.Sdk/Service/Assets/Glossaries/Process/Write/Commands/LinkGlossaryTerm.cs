using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Glossaries.Write
{
    public class LinkGlossaryTerm : Command
    {
        public Guid RelationshipIdentifier { get; set; }
        public Guid TermIdentifier { get; set; }
        public Guid ContainerIdentifier { get; set; }
        public string ContainerType { get; set; }
        public string ContentLabel { get; set; }

        public LinkGlossaryTerm(Guid glossary, Guid relationship, Guid term, Guid containerId, string containerType, string label)
        {
            AggregateIdentifier = glossary;
            RelationshipIdentifier = relationship;
            TermIdentifier = term;
            ContainerIdentifier = containerId;
            ContainerType = containerType;
            ContentLabel = label;
        }
    }
}
