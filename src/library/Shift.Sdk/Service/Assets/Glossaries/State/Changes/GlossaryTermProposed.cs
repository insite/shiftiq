using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Glossaries
{
    public class GlossaryTermProposed : Change
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public ContentContainer Content { get; set; }

        public GlossaryTermProposed(Guid id, string name, ContentContainer content)
        {
            Identifier = id;
            Name = name;
            Content = content;
        }
    }
}