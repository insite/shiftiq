using System;

namespace InSite.Domain.Glossaries
{
    public class GlossaryItemContent
    {
        public Guid Identifier { get; set; }
        public Guid Container { get; set; }
        public string ContentLabel { get; set; }
    }
}
