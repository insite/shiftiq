using System;

namespace InSite.Application.Glossaries.Read
{
    [Serializable]
    public class QGlossaryTermContent
    {
        public Guid RelationshipIdentifier { get; set; }
        public Guid TermIdentifier { get; set; }
        public string ContainerType { get; set; }
        public Guid ContainerIdentifier { get; set; }
        public string ContentLabel { get; set; }

        public virtual QGlossaryTerm Term { get; set; }
    }
}
