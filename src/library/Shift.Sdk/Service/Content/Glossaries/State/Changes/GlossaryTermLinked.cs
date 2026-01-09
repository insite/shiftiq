using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Glossaries
{
    public class GlossaryTermLinked : Change
    {
        public Guid RelationshipId { get; set; }
        public Guid TermId { get; set; }
        public Guid ContainerId { get; set; }
        public string ContainerType { get; set; }
        public string ContentLabel { get; set; }

        public GlossaryTermLinked(Guid relationship, Guid term, Guid containerId, string containerType, string label)
        {
            RelationshipId = relationship;
            TermId = term;
            ContainerId = containerId;
            ContainerType = containerType;
            ContentLabel = label;
        }
    }
}
