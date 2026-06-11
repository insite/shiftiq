using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Glossaries.Write
{
    public class ProposeGlossaryTerm : Command
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public ContentContainer Content { get; set; }

        public ProposeGlossaryTerm(Guid glossary, Guid term, string name, ContentContainer content)
        {
            AggregateIdentifier = glossary;
            Identifier = term;
            Name = name;
            Content = content;
        }
    }
}