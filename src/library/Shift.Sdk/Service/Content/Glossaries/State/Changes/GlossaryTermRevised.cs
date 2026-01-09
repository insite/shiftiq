using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Glossaries
{
    public class GlossaryTermRevised : Change
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public ContentContainer Content { get; set; }

        public GlossaryTermRevised(Guid identifier, string name, ContentContainer content)
        {
            Identifier = identifier;
            Name = name;
            Content = content;
        }
    }
}