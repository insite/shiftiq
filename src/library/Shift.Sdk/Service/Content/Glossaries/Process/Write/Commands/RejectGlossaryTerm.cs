using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Glossaries.Write
{
    public class RejectGlossaryTerm : Command
    {
        public Guid Term { get; set; }

        public RejectGlossaryTerm(Guid glossary, Guid term)
        {
            AggregateIdentifier = glossary;
            Term = term;
        }
    }
}