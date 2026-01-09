using System;

using Shift.Common;

namespace InSite.Application.Glossaries.Read
{
    [Serializable]
    public class GlossaryTermContentFilter : Filter
    {
        public Guid? GlossaryIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? ContainerIdentifier { get; set; }
        public Guid? TermIdentifier { get; set; }

        public string ContentLabel { get; set; }
        public string TermKeyword { get; set; }
    }
}
