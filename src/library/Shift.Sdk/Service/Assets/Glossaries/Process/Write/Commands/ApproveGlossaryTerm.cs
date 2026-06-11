using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Glossaries.Write
{
    public class ApproveGlossaryTerm : Command
    {
        public Guid Term { get; set; }

        public ApproveGlossaryTerm(Guid glossary, Guid term)
        {
            AggregateIdentifier = glossary;
            Term = term;
        }
    }
}